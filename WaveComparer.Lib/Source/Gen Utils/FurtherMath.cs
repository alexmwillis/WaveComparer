using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib
{
    enum WindowType
    {
        Rectangle,
        Hamming
    }

    struct GlobalVar
    {
        public int groupOffset, dataOffset;
        public int adr, groupNo, dataNo, blockNo, twNo;
        public double tw_re, tw_im, omega;
        public double[] twiddleRe , twiddleIm;
        public double[] trigRe , trigIm;
        public double[] zRe , zIm;
        public double[] vRe , vIm;
        public double[] wRe , wIm;

        public GlobalVar(int maxFactorCount, int maxPrimeFactorDiv2)
        {
            groupOffset = 0;
            dataOffset = 0;
            adr = 0;
            groupNo = 0;
            dataNo = 0;
            blockNo = 0;
            twNo = 0;
            omega = 0;
            tw_re = 0;
            tw_im = 0;

            twiddleRe = new double[maxFactorCount];
            twiddleIm = new double[maxFactorCount];
            trigRe = new double[maxFactorCount];
            trigIm = new double[maxFactorCount];
            zRe = new double[maxFactorCount];
            zIm = new double[maxFactorCount];
            vRe = new double[maxPrimeFactorDiv2];
            vIm = new double[maxPrimeFactorDiv2];
            wRe = new double[maxPrimeFactorDiv2];
            wIm = new double[maxPrimeFactorDiv2];
        }
    }

    static class FurtherMath
    {    
        public const int MaxPrimeFactor = 37;
        public const int MaxPrimeFactorDiv2 = (MaxPrimeFactor+1) / 2;
        public const int MaxFactorCount = 20;

        // STFFT constants
        public const int WindowLength = 200;
        public const int HopSize = 100;
 
        const double pi = 3.1415926535897932;
                          
        static double  c3_1 = -1.5000000000000E+00;  /*  c3_1 = cos(2*pi/3)-1;          */
        static double  c3_2 =  8.6602540378444E-01;  /*  c3_2 = sin(2*pi/3);            */
                                          
        // static double  u5   =  1.2566370614359E+00;  /*  u5   = 2*pi/5;                 */
        static double  c5_1 = -1.2500000000000E+00;  /*  c5_1 = (cos(u5)+cos(2*u5))/2-1;*/
        static double  c5_2 =  5.5901699437495E-01;  /*  c5_2 = (cos(u5)-cos(2*u5))/2;  */
        static double  c5_3 = -9.5105651629515E-01;  /*  c5_3 = -sin(u5);               */
        static double  c5_4 = -1.5388417685876E+00;  /*  c5_4 = -(sin(u5)+sin(2*u5));   */
        static double  c5_5 =  3.6327126400268E-01;  /*  c5_5 = (sin(u5)-sin(2*u5));    */
        static double  c8   =  7.0710678118655E-01;  /*  c8 = 1/sqrt(2);    */
        
        static double[] getWindow(WindowType windowType, int length)
        {
            double[] window = new double[length];

            switch (windowType)
            {
                case WindowType.Rectangle:

                    for (int i = 0; i < length; i++)
                    {
                        i = 1;
                    }
                    break;

                case WindowType.Hamming:

                    for (int i = 0; i < length; i++)
                    {
                        window[i] = 0.54 - 0.46 * Math.Cos(2 * pi * i /(length - 1));
                    }
                    break;
            }
            return window;
        }


        static void factorize(int n, out int nFact, int[] fact)
        {
            int i,j,k;
            int nRadix = 6;
            int[] radices = {0,2,3,4,5,8,10};
            int[] factors = new int[MaxFactorCount];
            
            if (n==1)
            {
                j=1;
                factors[1]=1;
            }
            else j=0;
            i=nRadix;
            while ((n>1) && (i>0))
            {
              if ((n % radices[i]) == 0)
              {
                n=n / radices[i];
                j=j+1;
                factors[j]=radices[i];
              }
              else  i=i-1;
            }
            if (factors[j] == 2)   /*substitute factors 2*8 with 4*4 */
            {   
              i = j-1;
              while ((i>0) && (factors[i] != 8)) i--;
              if (i>0)
              {
                factors[j] = 4;
                factors[i] = 4;
              }
            }
            if (n>1)
            {
                for (k=2; k<Math.Sqrt(n)+1; k++)
                    while ((n % k) == 0)
                    {
                        n=n / k;
                        j=j+1;
                        factors[j]=k;
                    }
                if (n>1)
                {
                    j=j+1;
                    factors[j]=n;
                }
            }               
            for (i=1; i<=j; i++)         
            {
              fact[i] = factors[j-i+1];
            }
            nFact=j;
        }   /* factorize */

        /****************************************************************************
          After N is factored the parameters that control the stages are generated.
          For each stage we have:
            sofar   : the product of the radices so far.
            actual  : the radix handled in this stage.
            remain  : the product of the remaining radices.
         ****************************************************************************/

        static void transTableSetup(int[] sofar, int[] actual, int[] remain,
                                    out int nFact, int nPoints)
        {
            int i;

            factorize(nPoints, out nFact, actual);
            if (actual[1] > MaxPrimeFactor)
            {
                //printf("\nPrime factor of FFT length too large : %6d",actual[1]);
                //printf("\nPlease modify the value of maxPrimeFactor in mixfft.c");
                return;
            }
            remain[0] = nPoints;
            sofar[1] = 1;
            remain[1] = nPoints / actual[1];
            for (i = 2; i <= nFact; i++)
            {
                sofar[i]=sofar[i-1]*actual[i-1];
                remain[i]=remain[i-1] / actual[i];
            }
        }   /* transTableSetup */

        /****************************************************************************
          The sequence y is the permuted input sequence x so that the following
          transformations can be performed in-place, and the final result is the
          normal order.
         ****************************************************************************/

        static void permute(int nPoint, int nFact,
                     int[] fact, int[] remain,
                     double[] xRe, double[] xIm,
                     double[] yRe, double[] yIm)

        {
            int i,j,k;
            int[] count = new int[MaxFactorCount]; 
            
            k=0;
            for (i=0; i<=nPoint-2; i++)
            {
                yRe[i] = xRe[k];
                yIm[i] = xIm[k];
                j=1;
                k=k+remain[j];
                count[1] = count[1]+1;
                while (count[j] >= fact[j])
                {
                    count[j]=0;
                    k=k-remain[j-1]+remain[j+1];
                    j=j+1;
                    count[j]=count[j]+1;
                }
            }
            yRe[nPoint-1]=xRe[nPoint-1];
            yIm[nPoint-1]=xIm[nPoint-1];
        }   /* permute */

        /****************************************************************************
          Twiddle factor multiplications and transformations are performed on a
          group of data. The number of multiplications with 1 are reduced by skipping
          the twiddle multiplication of the first stage and of the first group of the
          following stages.
         ***************************************************************************/

        static void initTrig(int radix, GlobalVar gv)
        {
            int i;
            double w, xre, xim;

            w = 2 * pi / radix;
            gv.trigRe[0] = 1;
            gv.trigIm[0] = 0;
            xre = Math.Cos(w);
            xim = -Math.Sin(w);
            gv.trigRe[1] = xre; gv.trigIm[1] = xim;
            for (i = 2; i < radix; i++)
            {
                gv.trigRe[i] = xre * gv.trigRe[i - 1] - xim * gv.trigIm[i - 1];
                gv.trigIm[i] = xim * gv.trigRe[i - 1] + xre * gv.trigIm[i - 1];
            }
        }   /* initTrig */

        static void fft_4(double[] aRe, double[] aIm)
        {
            double  t1_re,t1_im, t2_re,t2_im;
            double  m2_re,m2_im, m3_re,m3_im;

            t1_re=aRe[0] + aRe[2]; t1_im=aIm[0] + aIm[2];
            t2_re=aRe[1] + aRe[3]; t2_im=aIm[1] + aIm[3];

            m2_re=aRe[0] - aRe[2]; m2_im=aIm[0] - aIm[2];
            m3_re=aIm[1] - aIm[3]; m3_im=aRe[3] - aRe[1];

            aRe[0]=t1_re + t2_re; aIm[0]=t1_im + t2_im;
            aRe[2]=t1_re - t2_re; aIm[2]=t1_im - t2_im;
            aRe[1]=m2_re + m3_re; aIm[1]=m2_im + m3_im;
            aRe[3]=m2_re - m3_re; aIm[3]=m2_im - m3_im;
        }   /* fft_4 */


        static void fft_5(double[] aRe, double[] aIm)
        {    
            double  t1_re,t1_im, t2_re,t2_im, t3_re,t3_im;
            double  t4_re,t4_im, t5_re,t5_im;
            double  m2_re,m2_im, m3_re,m3_im, m4_re,m4_im;
            double  m1_re,m1_im, m5_re,m5_im;
            double  s1_re,s1_im, s2_re,s2_im, s3_re,s3_im;
            double  s4_re,s4_im, s5_re,s5_im;

            t1_re=aRe[1] + aRe[4]; t1_im=aIm[1] + aIm[4];
            t2_re=aRe[2] + aRe[3]; t2_im=aIm[2] + aIm[3];
            t3_re=aRe[1] - aRe[4]; t3_im=aIm[1] - aIm[4];
            t4_re=aRe[3] - aRe[2]; t4_im=aIm[3] - aIm[2];
            t5_re=t1_re + t2_re; t5_im=t1_im + t2_im;
            aRe[0]=aRe[0] + t5_re; aIm[0]=aIm[0] + t5_im;
            m1_re=c5_1*t5_re; m1_im=c5_1*t5_im;
            m2_re=c5_2*(t1_re - t2_re); m2_im=c5_2*(t1_im - t2_im);

            m3_re=-c5_3*(t3_im + t4_im); m3_im=c5_3*(t3_re + t4_re);
            m4_re=-c5_4*t4_im; m4_im=c5_4*t4_re;
            m5_re=-c5_5*t3_im; m5_im=c5_5*t3_re;

            s3_re=m3_re - m4_re; s3_im=m3_im - m4_im;
            s5_re=m3_re + m5_re; s5_im=m3_im + m5_im;
            s1_re=aRe[0] + m1_re; s1_im=aIm[0] + m1_im;
            s2_re=s1_re + m2_re; s2_im=s1_im + m2_im;
            s4_re=s1_re - m2_re; s4_im=s1_im - m2_im;

            aRe[1]=s2_re + s3_re; aIm[1]=s2_im + s3_im;
            aRe[2]=s4_re + s5_re; aIm[2]=s4_im + s5_im;
            aRe[3]=s4_re - s5_re; aIm[3]=s4_im - s5_im;
            aRe[4]=s2_re - s3_re; aIm[4]=s2_im - s3_im;
        }   /* fft_5 */

        static void fft_8(GlobalVar gv)
        {
            double[] aRe = new double[4];
            double[] aIm = new double[4];
            double[] bRe =  new double[4];
            double[] bIm =  new double[4];
            double gem;

            aRe[0] = gv.zRe[0];    bRe[0] = gv.zRe[1];
            aRe[1] = gv.zRe[2];    bRe[1] = gv.zRe[3];
            aRe[2] = gv.zRe[4];    bRe[2] = gv.zRe[5];
            aRe[3] = gv.zRe[6];    bRe[3] = gv.zRe[7];

            aIm[0] = gv.zIm[0]; bIm[0] = gv.zIm[1];
            aIm[1] = gv.zIm[2]; bIm[1] = gv.zIm[3];
            aIm[2] = gv.zIm[4]; bIm[2] = gv.zIm[5];
            aIm[3] = gv.zIm[6]; bIm[3] = gv.zIm[7];

            fft_4(aRe, aIm); fft_4(bRe, bIm);

            gem    = c8*(bRe[1] + bIm[1]);
            bIm[1] = c8*(bIm[1] - bRe[1]);
            bRe[1] = gem;
            gem    = bIm[2];
            bIm[2] =-bRe[2];
            bRe[2] = gem;
            gem    = c8*(bIm[3] - bRe[3]);
            bIm[3] =-c8*(bRe[3] + bIm[3]);
            bRe[3] = gem;
    
            gv.zRe[0] = aRe[0] + bRe[0]; gv.zRe[4] = aRe[0] - bRe[0];
            gv.zRe[1] = aRe[1] + bRe[1]; gv.zRe[5] = aRe[1] - bRe[1];
            gv.zRe[2] = aRe[2] + bRe[2]; gv.zRe[6] = aRe[2] - bRe[2];
            gv.zRe[3] = aRe[3] + bRe[3]; gv.zRe[7] = aRe[3] - bRe[3];

            gv.zIm[0] = aIm[0] + bIm[0]; gv.zIm[4] = aIm[0] - bIm[0];
            gv.zIm[1] = aIm[1] + bIm[1]; gv.zIm[5] = aIm[1] - bIm[1];
            gv.zIm[2] = aIm[2] + bIm[2]; gv.zIm[6] = aIm[2] - bIm[2];
            gv.zIm[3] = aIm[3] + bIm[3]; gv.zIm[7] = aIm[3] - bIm[3];
        }   /* fft_8 */

        static void fft_10(GlobalVar gv)
        {
            double[] aRe = new double[5];
            double[] aIm = new double[5];
            double[] bRe = new double[5];
            double[] bIm = new double[5];

            aRe[0] = gv.zRe[0];    bRe[0] = gv.zRe[5];
            aRe[1] = gv.zRe[2];    bRe[1] = gv.zRe[7];
            aRe[2] = gv.zRe[4];    bRe[2] = gv.zRe[9];
            aRe[3] = gv.zRe[6];    bRe[3] = gv.zRe[1];
            aRe[4] = gv.zRe[8];    bRe[4] = gv.zRe[3];

            aIm[0] = gv.zIm[0]; bIm[0] = gv.zIm[5];
            aIm[1] = gv.zIm[2]; bIm[1] = gv.zIm[7];
            aIm[2] = gv.zIm[4]; bIm[2] = gv.zIm[9];
            aIm[3] = gv.zIm[6]; bIm[3] = gv.zIm[1];
            aIm[4] = gv.zIm[8]; bIm[4] = gv.zIm[3];

            fft_5(aRe, aIm); fft_5(bRe, bIm);

            gv.zRe[0] = aRe[0] + bRe[0]; gv.zRe[5] = aRe[0] - bRe[0];
            gv.zRe[6] = aRe[1] + bRe[1]; gv.zRe[1] = aRe[1] - bRe[1];
            gv.zRe[2] = aRe[2] + bRe[2]; gv.zRe[7] = aRe[2] - bRe[2];
            gv.zRe[8] = aRe[3] + bRe[3]; gv.zRe[3] = aRe[3] - bRe[3];
            gv.zRe[4] = aRe[4] + bRe[4]; gv.zRe[9] = aRe[4] - bRe[4];

            gv.zIm[0] = aIm[0] + bIm[0]; gv.zIm[5] = aIm[0] - bIm[0];
            gv.zIm[6] = aIm[1] + bIm[1]; gv.zIm[1] = aIm[1] - bIm[1];
            gv.zIm[2] = aIm[2] + bIm[2]; gv.zIm[7] = aIm[2] - bIm[2];
            gv.zIm[8] = aIm[3] + bIm[3]; gv.zIm[3] = aIm[3] - bIm[3];
            gv.zIm[4] = aIm[4] + bIm[4]; gv.zIm[9] = aIm[4] - bIm[4];
        }   /* fft_10 */

        static void fft_odd(int radix, GlobalVar gv)
        {
            double  rere, reim, imre, imim;
            int     i,j,k,n,max;

            n = radix;
            max = (n + 1)/2;
            for (j=1; j < max; j++)
            {
                gv.vRe[j] = gv.zRe[j] + gv.zRe[n - j];
                gv.vIm[j] = gv.zIm[j] - gv.zIm[n - j];
                gv.wRe[j] = gv.zRe[j] - gv.zRe[n - j];
                gv.wIm[j] = gv.zIm[j] + gv.zIm[n - j];
            }

            for (j=1; j < max; j++)
            {
                gv.zRe[j] = gv.zRe[0];
                gv.zIm[j] = gv.zIm[0];
                gv.zRe[n - j] = gv.zRe[0];
                gv.zIm[n - j] = gv.zIm[0];
                k=j;
                for (i=1; i < max; i++)
                {
                    rere = gv.trigRe[k] * gv.vRe[i];
                    imim = gv.trigIm[k] * gv.vIm[i];
                    reim = gv.trigRe[k] * gv.wIm[i];
                    imre = gv.trigIm[k] * gv.wRe[i];

                    gv.zRe[n - j] += rere + imim;
                    gv.zIm[n - j] += reim - imre;
                    gv.zRe[j] += rere - imim;
                    gv.zIm[j] += reim + imre;

                    k = k + j;
                    if (k >= n)  k = k - n;
                }
            }
            for (j=1; j < max; j++)
            {
                gv.zRe[0] = gv.zRe[0] + gv.vRe[j];
                gv.zIm[0] = gv.zIm[0] + gv.wIm[j];
            }
        }   /* fft_odd */

        static void twiddleTransf(int sofarRadix, int radix, int remainRadix,
                    double[] yRe, double[] yIm, GlobalVar gv)

        {   /* twiddleTransf */ 
            double  cosw, sinw, gem;
            double  t1_re,t1_im, t2_re,t2_im, t3_re,t3_im;
            double  t4_re,t4_im, t5_re,t5_im;
            double  m2_re,m2_im, m3_re,m3_im, m4_re,m4_im;
            double  m1_re,m1_im, m5_re,m5_im;
            double  s1_re,s1_im, s2_re,s2_im, s3_re,s3_im;
            double  s4_re,s4_im, s5_re,s5_im;


            initTrig(radix, gv);
            gv.omega = 2 * pi / (double)(sofarRadix * radix);
            cosw = Math.Cos(gv.omega);
            sinw = -Math.Sin(gv.omega);
            gv.tw_re = 1.0;
            gv.tw_im = 0;
            gv.dataOffset = 0;
            gv.groupOffset = gv.dataOffset;
            gv.adr = gv.groupOffset;
            for (gv.dataNo = 0; gv.dataNo < sofarRadix; gv.dataNo++)
            {
                if (sofarRadix>1)
                {
                    gv.twiddleRe[0] = 1.0;
                    gv.twiddleIm[0] = 0.0;
                    gv.twiddleRe[1] = gv.tw_re;
                    gv.twiddleIm[1] = gv.tw_im;
                    for (gv.twNo = 2; gv.twNo < radix; gv.twNo++)
                    {
                        gv.twiddleRe[gv.twNo] = gv.tw_re * gv.twiddleRe[gv.twNo - 1]
                                       - gv.tw_im * gv.twiddleIm[gv.twNo - 1];
                        gv.twiddleIm[gv.twNo] = gv.tw_im * gv.twiddleRe[gv.twNo - 1]
                                       + gv.tw_re * gv.twiddleIm[gv.twNo - 1];
                    }
                    gem = cosw * gv.tw_re - sinw * gv.tw_im;
                    gv.tw_im = sinw * gv.tw_re + cosw * gv.tw_im;
                    gv.tw_re = gem;                      
                }
                for (gv.groupNo = 0; gv.groupNo < remainRadix; gv.groupNo++)
                {
                    if ((sofarRadix > 1) && (gv.dataNo > 0))
                    {
                        gv.zRe[0] = yRe[gv.adr];
                        gv.zIm[0] = yIm[gv.adr];
                        gv.blockNo = 1;
                        do {
                            gv.adr = gv.adr + sofarRadix;
                            gv.zRe[gv.blockNo] = gv.twiddleRe[gv.blockNo] * yRe[gv.adr]
                                         - gv.twiddleIm[gv.blockNo] * yIm[gv.adr];
                            gv.zIm[gv.blockNo] = gv.twiddleRe[gv.blockNo] * yIm[gv.adr]
                                         + gv.twiddleIm[gv.blockNo] * yRe[gv.adr];

                            gv.blockNo++;
                        } while (gv.blockNo < radix);
                    }
                    else
                        for (gv.blockNo = 0; gv.blockNo < radix; gv.blockNo++)
                        {
                            gv.zRe[gv.blockNo] = yRe[gv.adr];
                            gv.zIm[gv.blockNo] = yIm[gv.adr];
                            gv.adr = gv.adr + sofarRadix;
                        }
                    switch(radix) {
                        case 2:
                            gem = gv.zRe[0] + gv.zRe[1];
                            gv.zRe[1] = gv.zRe[0] - gv.zRe[1];
                            gv.zRe[0] = gem;
                            gem = gv.zIm[0] + gv.zIm[1];
                            gv.zIm[1] = gv.zIm[0] - gv.zIm[1];
                            gv.zIm[0] = gem;
                            break;

                        case  3:
                            t1_re = gv.zRe[1] + gv.zRe[2];
                            t1_im = gv.zIm[1] + gv.zIm[2];
                            gv.zRe[0] = gv.zRe[0] + t1_re;
                            gv.zIm[0] = gv.zIm[0] + t1_im;
                            m1_re = c3_1 * t1_re;
                            m1_im = c3_1 * t1_im;
                            m2_re = c3_2 * (gv.zIm[1] - gv.zIm[2]);
                            m2_im = c3_2 * (gv.zRe[2] - gv.zRe[1]);
                            s1_re = gv.zRe[0] + m1_re;
                            s1_im = gv.zIm[0] + m1_im;
                            gv.zRe[1] = s1_re + m2_re;
                            gv.zIm[1] = s1_im + m2_im;
                            gv.zRe[2] = s1_re - m2_re;
                            gv.zIm[2] = s1_im - m2_im;
                            break;

                        case  4:
                            t1_re = gv.zRe[0] + gv.zRe[2];
                            t1_im = gv.zIm[0] + gv.zIm[2];
                            t2_re = gv.zRe[1] + gv.zRe[3];
                            t2_im = gv.zIm[1] + gv.zIm[3];
                            m2_re = gv.zRe[0] - gv.zRe[2];
                            m2_im = gv.zIm[0] - gv.zIm[2];
                            m3_re = gv.zIm[1] - gv.zIm[3];
                            m3_im = gv.zRe[3] - gv.zRe[1];
                            gv.zRe[0] = t1_re + t2_re;
                            gv.zIm[0] = t1_im + t2_im;
                            gv.zRe[2] = t1_re - t2_re;
                            gv.zIm[2] = t1_im - t2_im;
                            gv.zRe[1] = m2_re + m3_re;
                            gv.zIm[1] = m2_im + m3_im;
                            gv.zRe[3] = m2_re - m3_re;
                            gv.zIm[3] = m2_im - m3_im;
                            break;

                        case  5:
                            t1_re = gv.zRe[1] + gv.zRe[4];
                            t1_im = gv.zIm[1] + gv.zIm[4];
                            t2_re = gv.zRe[2] + gv.zRe[3];
                            t2_im = gv.zIm[2] + gv.zIm[3];
                            t3_re = gv.zRe[1] - gv.zRe[4];
                            t3_im = gv.zIm[1] - gv.zIm[4];
                            t4_re = gv.zRe[3] - gv.zRe[2];
                            t4_im = gv.zIm[3] - gv.zIm[2];
                            t5_re = t1_re + t2_re;
                            t5_im = t1_im + t2_im;
                            gv.zRe[0] = gv.zRe[0] + t5_re;
                            gv.zIm[0] = gv.zIm[0] + t5_im;
                            m1_re = c5_1 * t5_re;
                            m1_im = c5_1 * t5_im;
                            m2_re = c5_2 * (t1_re - t2_re);
                            m2_im = c5_2 * (t1_im - t2_im);
                            m3_re = -c5_3 * (t3_im + t4_im);
                            m3_im = c5_3 * (t3_re + t4_re);
                            m4_re = -c5_4 * t4_im;
                            m4_im = c5_4 * t4_re;
                            m5_re = -c5_5 * t3_im;
                            m5_im = c5_5 * t3_re;
                            s3_re = m3_re - m4_re;
                            s3_im = m3_im - m4_im;
                            s5_re = m3_re + m5_re;
                            s5_im = m3_im + m5_im;
                            s1_re = gv.zRe[0] + m1_re;
                            s1_im = gv.zIm[0] + m1_im;
                            s2_re = s1_re + m2_re;
                            s2_im = s1_im + m2_im;
                            s4_re = s1_re - m2_re;
                            s4_im = s1_im - m2_im;

                            gv.zRe[1] = s2_re + s3_re;
                            gv.zIm[1] = s2_im + s3_im;
                            gv.zRe[2] = s4_re + s5_re;
                            gv.zIm[2] = s4_im + s5_im;
                            gv.zRe[3] = s4_re - s5_re;
                            gv.zIm[3] = s4_im - s5_im;
                            gv.zRe[4] = s2_re - s3_re;
                            gv.zIm[4] = s2_im - s3_im;
                            break;

                      case  8  : fft_8(gv); break;
                      case 10  : fft_10(gv); break;
                      default  : fft_odd(radix, gv); break;
                    }
                    gv.adr = gv.groupOffset;
                    for (gv.blockNo = 0; gv.blockNo < radix; gv.blockNo++)
                    {
                        yRe[gv.adr] = gv.zRe[gv.blockNo]; yIm[gv.adr] = gv.zIm[gv.blockNo];
                        gv.adr = gv.adr + sofarRadix;
                    }
                    gv.groupOffset = gv.groupOffset + sofarRadix * radix;
                    gv.adr = gv.groupOffset;
                }
                gv.dataOffset = gv.dataOffset + 1;
                gv.groupOffset = gv.dataOffset;
                gv.adr = gv.groupOffset;
            }
        }   /* twiddleTransf */
        
        /// <summary>
        /// Fast Fourier transform
        /// </summary>
        /// <param name="n">FTT size</param>
        /// <param name="xRe"></param>
        /// <param name="xIm"></param>
        /// <param name="yRe"></param>
        /// <param name="yIm"></param>
        public static void fft(int n, double[] xRe, double[] xIm,
                out double[] yRe, out double[] yIm)
        {
            var gv = new GlobalVar(MaxFactorCount, MaxPrimeFactorDiv2);
            int[] sofarRadix = new int[MaxFactorCount];
            int[] actualRadix = new int[MaxFactorCount]; 
            int[] remainRadix = new int[MaxFactorCount];
            int nFactor = 0;
            int count;

            yRe = new double[n];
            yIm = new double[n];

            transTableSetup(sofarRadix, actualRadix, remainRadix, out nFactor, n);
            permute(n, nFactor, actualRadix, remainRadix, xRe, xIm, yRe, yIm);

            for (count=1; count<=nFactor; count++)
              twiddleTransf(sofarRadix[count], actualRadix[count], remainRadix[count], 
                            yRe, yIm, gv);
        }   /* fft */

        /// <summary>
        ///  Short Time Fourier Transform. 
        /// </summary>
        /// <param name="N">Resolution of fourier transform</param>
        /// <param name="hop">Number of samples to hop forward</param>
        /// <param name="xRe">Real component of input signal</param>
        /// <param name="xIm">Imaginary component of input signal</param>
        /// <param name="yRe">Real component of output signal</param>
        /// <param name="yIm">Imaginary component of output signal</param>
        public static void stfft(int N, double[] xRe, double[] xIm,
                                 out double[][] yRe, out double[][] yIm)
        {
            stfft(N, FurtherMath.WindowLength, HopSize, xRe, xIm, out yRe, out yIm, -1);
        }

        /// <summary>
        ///  Short Time Fourier Transform. 
        /// </summary>
        /// <param name="N">Resolution of fourier transform</param>
        /// <param name="hop">Number of samples to hop forward</param>
        /// <param name="xRe">Real component of input signal</param>
        /// <param name="xIm">Imaginary component of input signal</param>
        /// <param name="yRe">Real component of output signal</param>
        /// <param name="yIm">Imaginary component of output signal</param>
        /// /// <param name="numberOfFrames">Number of frames to process</param>
        public static void stfft(int N, double[] xRe, double[] xIm,
                                 out double[][] yRe, out double[][] yIm, int numberOfFrames)
        {
            stfft(N, FurtherMath.WindowLength, HopSize, xRe, xIm, out yRe, out yIm, numberOfFrames);
        }
        /// <summary>
        ///  Short Time Fourier Transform. 
        /// </summary>
        /// <param name="N">Resolution of fourier transform</param>
        /// <param name="M">Number of samples in window</param>
        /// <param name="hop">Number of samples to hop forward</param>
        /// <param name="xRe">Real component of input signal</param>
        /// <param name="xIm">Imaginary component of input signal</param>
        /// <param name="yRe">Real component of output signal</param>
        /// <param name="yIm">Imaginary component of output signal</param>
        /// <param name="numberOfFrames">Number of frames to process</param>
        public static void stfft(int N, int M, int hop, double[] xRe, double[] xIm,
                                 out double[][] yRe, out double[][] yIm, int numberOfFrames)
        {            
            int Mo2 = (int)M/2;
            
            int n = (int)Math.Ceiling((double)xRe.Length/hop); // number of frames in the sample
            if (numberOfFrames < 0)
            {
                numberOfFrames = n;
            }
            double[] window = getWindow(WindowType.Hamming, M);
            double[] xwRe = new double[M]; // windowed segment
                        
            double[] xwzIm = new double[N];

            yRe = new double[numberOfFrames][];
            yIm = new double[numberOfFrames][];

            for (int i = 0; i < numberOfFrames; i++)
            {
                if (i >= n)
                {
                    // If numberOfFrames > n just initialise these frames 
                    yRe[i] = new double[N];
                    yIm[i] = new double[N];
                }
                else
                {                    
                    // Populate windowed frame (only implemented for real part)
                    for (int j = 0; j < xwRe.Length; j++)
                    {
                        int xIndex = j + (i * hop) - Mo2;
                        if (xIndex >= 0 && xIndex < xRe.Length)
                        {
                            xwRe[j] = xRe[xIndex] * window[j];
                        }
                        else
                        {
                            // Zero padding
                            xwRe[j] = 0;
                        }
                    }
                    double[] xwzRe = new double[N]; // windowed zero-padded segment
                    Array.Copy(xwRe, Mo2, xwzRe, 0, Mo2);
                    Array.Copy(xwRe, 0, xwzRe, N - Mo2, Mo2);

                    fft(N, xwzRe, xwzIm, out yRe[i], out yIm[i]);
                }
            }
        }

        public static double[] Amplitude(double[] real, double[] imaginary)
        {
            double[] Amp = new double[real.Length];
            if (real.Length == imaginary.Length) 
            {            
                for (int i = 0; i < real.Length; i++) 
                {
                    Amp[i] = Math.Sqrt(real[i]*real[i] + imaginary[i]*imaginary[i]);
                }
            }
            return Amp;
        }

        public static double GetCovariance(double[] X, double[] Y)
        {
            int n = X.Length;
            // Get average
            double E_X = X.Average();
            double E_Y = Y.Average();
            double covariance = 0;
            for (int i = 0; i < n; i++)
            {
                covariance += (X[i] - E_X) * (Y[i] - E_Y) / n;
            }
            return covariance;            
        }

        public static double GetVariance(double[] X)
        {
            return GetCovariance(X, X);
        }

        public static int GetIndexOfMax(double[] array)
        {
            double maxValue = 0;
            int maxIndex = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > maxValue)
                {
                    maxValue = array[i];
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        //public static void CentraliseVolume(double[][] data)
        //{
        //    // Centralises about x axis
        //    var areas = new double[data[0].Length];
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        for (int j = 0; j < data[i].Length; j++)
        //        {
        //            areas[j] += data[i][j];
        //        }
        //    }
        //    var halfVolume = areas.Sum() / 2;
        //    for (int i = 1; i < areas.Length; i++)
        //    {
        //        areas[i] += areas[i - 1];
        //        if (areas[i] > halfVolume)
        //        {
        //            var centreOfVolume = i;
        //            break;
        //        }
        //    }
        //}

        public static double[] AverageArray(double[][] array)
        {
            // Averages on the first dimension
            var result = new double[array[0].Length];
            var n = array.Length;
            for (int i = 0; i < n; i++)
            {   
                for (int j = 0; j < array[i].Length; j++)
                {
                    result[j] += array[i][j] / n;
                }
            }
            return result;
        }

        public static int AreaMidPoint<T>(T[] array) where T : IConvertible
        {
            var halfTotal = array.Sum<T>((v) => Convert.ToDouble(v)) / 2;
            var index = 0;
            if (halfTotal == 0) throw new ArgumentException("Array is empty");
            double sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += Convert.ToDouble(array[i]);
                if (sum > halfTotal)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Logs each element in array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static double[] LogArray(double[] array)
        {
            return array.Select((a) => Math.Log(a)).ToArray();
        }
    }
}
