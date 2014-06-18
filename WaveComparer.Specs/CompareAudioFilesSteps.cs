using System;
using TechTalk.SpecFlow;

using WaveComparer.Lib;

namespace WaveComparer.Specs
{
    [Binding]
    public class CompareAudioFilesSteps
    {
        private AudioFile kickDrum1;
        private AudioFile kickDrum2;
        private AudioFile snare1;

        [Given(@"two kickdrums and a snare")]
        public void GivenTwoKickdrumsAndASnare()
        {

        }

        [When(@"when these samples are compared")]
        public void WhenWhenTheseSamplesAreCompared()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the distance between the two kick drums should be less than the distance between the kick drums and the snare")]
        public void ThenTheDistanceBetweenTheTwoKickDrumsShouldBeLessThanTheDistanceBetweenTheKickDrumsAndTheSnare()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
