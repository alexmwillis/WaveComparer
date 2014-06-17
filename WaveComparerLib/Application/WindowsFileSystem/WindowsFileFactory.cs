using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveComparerLib.WindowsFileSystem
{
    public class WindowsFileFactory
    {
        public WindowsFile GetWindowsFile(FileInfo fileInfo)
        {
            WindowsFile file;
            if (fileInfo.Extension.ToUpper() == ".WAV")
            {
                file = new WindowsAudioFile(fileInfo);
            }
            else
            {
                throw new NotImplementedException();
            }
            // Hookup actions 
            // TODO all actions are currently hooked up to all files, the actions themselves are responsible
            // for deciding whether they can use the parameter, but ideally selection would be done here
            file.Actions = WaveComparerToolBox.Instance.AvailableActions;

            //foreach (var action in WaveComparerToolBox.Instance.AvailableActions)
            //{
            //    if (file.PrimaryAction == null)
            //        file.PrimaryAction = action;
            //    else if (file.SecondaryAction == null)
            //        file.SecondaryAction = action;
            //    else
            //        break;
            //}
            return file;
        }
    }
}
