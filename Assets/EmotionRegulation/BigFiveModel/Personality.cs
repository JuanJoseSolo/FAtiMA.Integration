using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EmotionRegulation.BigFiveModel
{
    internal class Personality
    {
        float Openness;
        float Conscientiousness;
        float Extraversion;
        float Agreeableness;
        float Neuroticism;
        internal BigFiveModel BigFive { get; set; }

        public Personality(PersonalityDTO persoDTO)
        {
            Openness = persoDTO.Openness;
            Conscientiousness = persoDTO.Conscientiousness;
            Extraversion = persoDTO.Extraversion;
            Agreeableness = persoDTO.Agreeableness;
            Neuroticism = persoDTO.Neuroticism;
            SetPersonality();
        }

        public void SetPersonality()
        {
            List<float> checkList = new List<float>() { Openness, Conscientiousness, Extraversion, Agreeableness, Neuroticism };
            var NotNull = checkList.Any(p => p != 0);
            if (NotNull)
            {
                BigFive = new BigFiveModel(
                          Openness, 
                          Conscientiousness, 
                          Extraversion, 
                          Agreeableness, 
                          Neuroticism);
            }
            else Debug.Print("ArgumentNullException");
        }
    }
}
