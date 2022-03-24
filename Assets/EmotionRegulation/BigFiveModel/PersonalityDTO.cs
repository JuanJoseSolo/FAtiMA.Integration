using System;
using System.Collections.Generic;
using System.Text;

namespace EmotionRegulation.BigFiveModel
{
    public class PersonalityDTO
    {
        public float Openness { get => openness; set => openness = value; }
        public float Conscientiousness { get => conscientiousness; set => conscientiousness = value; }
        public float Extraversion { get => extraversion; set => extraversion = value; }
        public float Agreeableness { get => agreeableness; set => agreeableness = value; }
        public float Neuroticism { get => neuroticism; set => neuroticism = value; }
        public float MaxLevelEmotion { get => maxLevelEmotion; set => maxLevelEmotion = value; }

        float openness = 0;
        float conscientiousness = 0;
        float extraversion = 0;
        float agreeableness = 0;
        float neuroticism = 0;
        float maxLevelEmotion = 0;

        public List<float> ToList()
        {
            List<float> personality_List = new List<float>()
            {
                this.openness, this.conscientiousness, this.extraversion, this.agreeableness, this.neuroticism
            };

            return personality_List;
        }
    }


    
}
