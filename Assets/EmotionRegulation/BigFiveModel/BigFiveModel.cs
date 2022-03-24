using System;
using System.Collections.Generic;
using System.Linq;
using EmotionRegulation.Components;
using FLS;

namespace EmotionRegulation.BigFiveModel
{


    public class BigFiveModel
    {

        public List<string> AllPersonalities { get; private set; }
        public string DominantPersonality { get; private set; }
        public List<KeyValuePair<string, string>> StrategyMetrics { get; private set; }
        public List<string> StrategiesToApply { get; private set; }


        LinguisticVariable Linguistic_Openness;
        LinguisticVariable Linguistic_Conscientiousness;
        LinguisticVariable Linguistic_Extraversion;
        LinguisticVariable Linguistic_Neuroticism;
        LinguisticVariable Linguistic_Agreeableness;
        FLS.MembershipFunctions.IMembershipFunction low_Openness;
        FLS.MembershipFunctions.IMembershipFunction middle_Openness;
        FLS.MembershipFunctions.IMembershipFunction high_Openness;
        FLS.MembershipFunctions.IMembershipFunction low_Conscientiousness;
        FLS.MembershipFunctions.IMembershipFunction middle_Conscientiousness;
        FLS.MembershipFunctions.IMembershipFunction high_Conscientiousness;
        FLS.MembershipFunctions.IMembershipFunction low_Extraversion;
        FLS.MembershipFunctions.IMembershipFunction middle_Extraversion;
        FLS.MembershipFunctions.IMembershipFunction high_Extraversion;
        FLS.MembershipFunctions.IMembershipFunction low_Agreeableness;
        FLS.MembershipFunctions.IMembershipFunction middle_Agreeableness;
        FLS.MembershipFunctions.IMembershipFunction high_Agreeableness;
        FLS.MembershipFunctions.IMembershipFunction low_Neuroticism;
        FLS.MembershipFunctions.IMembershipFunction middle_Neuroticism;
        FLS.MembershipFunctions.IMembershipFunction high_Neuroticism;

        float conscientiousness;
        float extraversion;
        float neuroticism;
        float openness;
        float agreeableness;

        public BigFiveModel() { }

        public BigFiveModel(float openness, float conscientiousness, float extraversion, float agreeableness, float neuroticism)
        {


            DominantPersonality = string.Empty;
            AllPersonalities = new List<string>();
            StrategyMetrics = new List<KeyValuePair<string, string>>();
            StrategiesToApply = new List<string>();

            this.conscientiousness = conscientiousness;
            this.extraversion = extraversion;
            this.neuroticism = neuroticism;
            this.openness = openness;
            this.agreeableness = agreeableness;

            PersonalityDerivation();
            StrategiesDerivation();

        }
        
        private void PersonalityDerivation()
        {
            const float Low = 1;
            const float Middle = 2;
            const float High = 3;

            List<(string trait, float weight)> personalityType = new List<(string trait, float weight)>();

            Linguistic_Conscientiousness = new LinguisticVariable("conscientiousness");
            Linguistic_Extraversion = new LinguisticVariable("extraversion");
            Linguistic_Neuroticism = new LinguisticVariable("neuroticism");
            Linguistic_Openness = new LinguisticVariable("openness");
            Linguistic_Agreeableness = new LinguisticVariable("agreeableness");

            low_Conscientiousness = Linguistic_Conscientiousness.MembershipFunctions.AddZShaped("lowConscientiousness", 30, 10, 0, 100);
            middle_Conscientiousness = Linguistic_Conscientiousness.MembershipFunctions.AddGaussian("middleConscientiousness", 50, 10, 0, 100);
            high_Conscientiousness = Linguistic_Conscientiousness.MembershipFunctions.AddSShaped("highConscientiousness", 70, 10, 0, 100);

            low_Extraversion = Linguistic_Extraversion.MembershipFunctions.AddZShaped("lowExtraversion", 30, 10, 0, 100);
            middle_Extraversion = Linguistic_Extraversion.MembershipFunctions.AddGaussian("middleExtraversion", 50, 10, 0, 100);
            high_Extraversion = Linguistic_Extraversion.MembershipFunctions.AddSShaped("highExtraversion", 70, 10, 0, 100);

            low_Neuroticism = Linguistic_Neuroticism.MembershipFunctions.AddZShaped("lowNeuroticism", 30, 10, 0, 100);
            middle_Neuroticism = Linguistic_Neuroticism.MembershipFunctions.AddGaussian("middleNeuroticism", 50, 10, 0, 100);
            high_Neuroticism = Linguistic_Neuroticism.MembershipFunctions.AddSShaped("highNeuroticism", 70, 10, 0, 100);

            low_Openness = Linguistic_Openness.MembershipFunctions.AddZShaped("lowOpenness", 30, 10, 0, 100);
            middle_Openness = Linguistic_Openness.MembershipFunctions.AddGaussian("middleOpenness", 50, 10, 0, 100);
            high_Openness = Linguistic_Openness.MembershipFunctions.AddSShaped("highOpenness", 70, 10, 0, 100);

            low_Agreeableness = Linguistic_Agreeableness.MembershipFunctions.AddZShaped("lowAgreeableness", 30, 10, 0, 100);
            middle_Agreeableness = Linguistic_Agreeableness.MembershipFunctions.AddGaussian("middleAgreeableness", 50, 10, 0, 100);
            high_Agreeableness = Linguistic_Agreeableness.MembershipFunctions.AddSShaped("highAgreeableness", 70, 10, 0, 100);


            Dictionary<string, float> Dic_PersonalityType1 = new Dictionary<string, float>()
            {
                { "Low Conscientiousness", (float)low_Conscientiousness.Fuzzify(conscientiousness) },
                { "Middle Conscientiousness", (float)middle_Conscientiousness.Fuzzify(conscientiousness) },
                { "High Conscientiousness", (float)high_Conscientiousness.Fuzzify(conscientiousness) },
            };
            var Personality_1 = Dic_PersonalityType1.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            if (Personality_1.Contains("Low")) personalityType.Add((Personality_1, Low));
            else if (Personality_1.Contains("Middle")) personalityType.Add((Personality_1, Middle));
            else personalityType.Add((Personality_1, High));

            Dictionary<string, float> Dic_PersonalityType2 = new Dictionary<string, float>()
            {
                { "Low Extraversion", (float)low_Extraversion.Fuzzify(extraversion) },
                { "Middle Extraversion", (float)middle_Extraversion.Fuzzify(extraversion) },
                { "High Extraversion", (float)high_Extraversion.Fuzzify(extraversion) },
            };
            var Personality_2 = Dic_PersonalityType2.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            if (Personality_2.Contains("Low")) personalityType.Add((Personality_2, Low));
            else if (Personality_2.Contains("Middle")) personalityType.Add((Personality_2, Middle));
            else personalityType.Add((Personality_2, High));


            Dictionary<string, float> Dic_PersonalityType3 = new Dictionary<string, float>()
            {
                { "Low Neuroticism", (float)low_Neuroticism.Fuzzify(neuroticism) },
                { "Middle Neuroticism", (float)middle_Neuroticism.Fuzzify(neuroticism) },
                { "High Neuroticism", (float)high_Neuroticism.Fuzzify(neuroticism) },
            };
            var Personality_3 = Dic_PersonalityType3.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            if (Personality_3.Contains("Low")) personalityType.Add((Personality_3, Low));
            else if (Personality_3.Contains("Middle")) personalityType.Add((Personality_3, Middle));
            else personalityType.Add((Personality_3, High));


            Dictionary<string, float> Dic_PersonalityType4 = new Dictionary<string, float>()
            {
                { "Low Agreeableness", (float)low_Agreeableness.Fuzzify(agreeableness) },
                { "Middle Agreeableness", (float)middle_Agreeableness.Fuzzify(agreeableness) },
                { "High Agreeableness", (float)high_Agreeableness.Fuzzify(agreeableness) },
            };
            var Personality_4 = Dic_PersonalityType4.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            if (Personality_4.Contains("Low")) personalityType.Add((Personality_4, Low));
            else if (Personality_4.Contains("Middle")) personalityType.Add((Personality_4, Middle));
            else personalityType.Add((Personality_4, High));

            Dictionary<string, float> PersonalityLinguisticResult5 = new Dictionary<string, float>()
            {
                { "Low Openness", (float)low_Openness.Fuzzify(openness) },
                { "Middle Openness", (float)middle_Openness.Fuzzify(openness) },
                { "High Openness", (float)high_Openness.Fuzzify(openness) },
            };
            var Personality_5 = PersonalityLinguisticResult5.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            if (Personality_5.Contains("Low")) personalityType.Add((Personality_5, Low));
            else if (Personality_5.Contains("Middle")) personalityType.Add((Personality_5, Middle));
            else personalityType.Add((Personality_5, High));


            DominantPersonality = personalityType.Aggregate((t, w)
                                                    => t.weight > w.weight ? t : w).trait.Split(' ')[1];
            //Return all personalities of the agent
            AllPersonalities = new List<string>();

            personalityType.ForEach(p => AllPersonalities.Add(p.trait));

        }

        public List<string> GetPersonalities(float openness, float conscientiousness, float extraversion, float agreeableness, float neuroticism)
        {
            this.openness = openness;
            this.conscientiousness = conscientiousness;
            this.extraversion = extraversion;
            this.agreeableness = agreeableness;
            this.neuroticism = neuroticism;

            PersonalityDerivation();

            return AllPersonalities;
        }
        
        private void StrategiesDerivation()
        {
            StrategyMetrics.Add(FuzzyfieSituationSelection());
            StrategyMetrics.Add(FuzzyfieSituationModification());
            StrategyMetrics.Add(FuzzyfieAttentionalDeployment());
            StrategyMetrics.Add(FuzzyfieCognitiveChange());
            StrategyMetrics.Add(FuzzyfieResponseModulation());

            StrategiesToApply = StrategyMetrics.Where(v => v.Value == "Strongly").Select(s => s.Key).ToList();
        }



        #region Strategies
        //Situation selection
        private KeyValuePair<string,string> FuzzyfieSituationSelection()
        {
            var SituationSelection = new LinguisticVariable("SituationSelection");
            var WeaklyApplied = SituationSelection.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = SituationSelection.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = SituationSelection.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            ///both personalities aren't opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(high_Conscientiousness).And(
                Linguistic_Neuroticism.Is(high_Neuroticism))).Then(SituationSelection.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness).And(
                Linguistic_Neuroticism.Is(middle_Neuroticism))).Then(SituationSelection.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(low_Conscientiousness).And(
                Linguistic_Neuroticism.Is(low_Neuroticism))).Then(SituationSelection.Is(WeaklyApplied));

            var rule4 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(high_Extraversion).And(
                Linguistic_Openness.Is(high_Openness)).And(
                Linguistic_Agreeableness.Is(high_Agreeableness))).Then(SituationSelection.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(middle_Extraversion).And(
                Linguistic_Openness.Is(middle_Openness)).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness))).Then(SituationSelection.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(low_Extraversion).And(
                Linguistic_Openness.Is(low_Openness)).And(
                Linguistic_Agreeableness.Is(low_Agreeableness))).Then(SituationSelection.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify =
                fuzzyEngine.Defuzzify(new
                {
                    conscientiousness = (double)conscientiousness,
                    extraversion = (double)extraversion,
                    neuroticism = (double)neuroticism,
                    agreeableness = (double)agreeableness,
                    openness = (double)openness
                });

            Dictionary<string, float> DstrategyLinguisticResult = new Dictionary<string, float>();

            DstrategyLinguisticResult.Add("Weakly", (float)WeaklyApplied.Fuzzify(OutputDefuzzify));
            DstrategyLinguisticResult.Add("Lightly", (float)LightlyApplied.Fuzzify(OutputDefuzzify));
            DstrategyLinguisticResult.Add("Strongly", (float)StronglyApplied.Fuzzify(OutputDefuzzify));

            var StrategyPower = DstrategyLinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            var StrategyResults = new KeyValuePair<string, string>( EmotionRegulationModel.SITUATION_SELECTION, StrategyPower);

            return StrategyResults;
        }

        //Situation modification
        private KeyValuePair<string,string> FuzzyfieSituationModification()
        {
            var SituationModification = new LinguisticVariable("SituationModification");
            var WeaklyApplied = SituationModification.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = SituationModification.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = SituationModification.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(high_Conscientiousness).And(
                Linguistic_Extraversion.Is(high_Extraversion)).And(
                Linguistic_Openness.Is(high_Openness))).Then(SituationModification.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness).And(
                Linguistic_Extraversion.Is(middle_Extraversion)).And(
                Linguistic_Openness.Is(middle_Openness))).Then(SituationModification.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(low_Conscientiousness).And(
                Linguistic_Extraversion.Is(low_Extraversion)).And(
                Linguistic_Openness.Is(low_Openness))).Then(SituationModification.Is(WeaklyApplied));

            //both strategies are not opposites each other
            var rule4 = fuzzyEngine.Rules.If(
                Linguistic_Neuroticism.Is(high_Neuroticism).And(
                Linguistic_Agreeableness.Is(high_Agreeableness))).Then(SituationModification.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                Linguistic_Neuroticism.Is(middle_Neuroticism).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness))).Then(SituationModification.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                Linguistic_Neuroticism.Is(low_Neuroticism).And(
                Linguistic_Agreeableness.Is(low_Agreeableness))).Then(SituationModification.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = fuzzyEngine.Defuzzify(new
            {
                conscientiousness = (double)conscientiousness,
                extraversion = (double)extraversion,
                neuroticism = (double)neuroticism,
                agreeableness = (double)agreeableness,
                openness = (double)openness
            });

            Dictionary<string, float> LinguisticResult = new Dictionary<string, float>()
            {
                { "Weakly", (float)WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", (float)LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", (float)StronglyApplied.Fuzzify(OutputDefuzzify) },
            };

            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            var StrategyResults = new KeyValuePair<string, string>(EmotionRegulationModel.SITUATION_MODIFICATION, StrategyPower);

            return StrategyResults;
        }

        ////Attention deployment
        private KeyValuePair<string, string> FuzzyfieAttentionalDeployment()
        {
            var AttentionalDeployment = new LinguisticVariable("AttentionalDeployment");
            var WeaklyApplied = AttentionalDeployment.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = AttentionalDeployment.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = AttentionalDeployment.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(high_Openness).And(
                Linguistic_Conscientiousness.Is(high_Conscientiousness)).And(
                Linguistic_Agreeableness.Is(high_Agreeableness)).And(
                Linguistic_Extraversion.Is(high_Extraversion))).Then(AttentionalDeployment.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(middle_Openness).And(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness)).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness)).And(
                Linguistic_Extraversion.Is(middle_Extraversion))).Then(AttentionalDeployment.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(low_Openness).And(
                Linguistic_Conscientiousness.Is(low_Conscientiousness)).And(
                Linguistic_Agreeableness.Is(low_Agreeableness)).And(
                Linguistic_Extraversion.Is(low_Extraversion))).Then(AttentionalDeployment.Is(WeaklyApplied));

            var rule4 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(high_Neuroticism)).Then(AttentionalDeployment.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(middle_Neuroticism)).Then(AttentionalDeployment.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(low_Neuroticism)).Then(AttentionalDeployment.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = fuzzyEngine.Defuzzify(new
            {
                conscientiousness = (double)conscientiousness,
                extraversion = (double)extraversion,
                neuroticism = (double)neuroticism,
                agreeableness = (double)agreeableness,
                openness = (double)openness
            });

            Dictionary<string, float> LinguisticResult = new Dictionary<string, float>()
            {
                { "Weakly", (float)WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", (float)LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", (float)StronglyApplied.Fuzzify(OutputDefuzzify) },
            };
            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            var StrategyResults = new KeyValuePair<string, string>(EmotionRegulationModel.ATTENTION_DEPLOYMENT, StrategyPower);

            return StrategyResults;
        }

        /////Cognitive change
        private KeyValuePair<string, string> FuzzyfieCognitiveChange()
        {
            var CognitiveChange = new LinguisticVariable("CognitiveChange");
            var WeaklyApplied = CognitiveChange.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = CognitiveChange.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = CognitiveChange.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(high_Neuroticism)).Then(CognitiveChange.Is(WeaklyApplied));
            var rule2 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(middle_Neuroticism)).Then(CognitiveChange.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(low_Neuroticism)).Then(CognitiveChange.Is(StronglyApplied));

            var rule4 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(high_Openness).And(
                Linguistic_Agreeableness.Is(high_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(high_Conscientiousness)).And(
                Linguistic_Extraversion.Is(high_Extraversion))).Then(CognitiveChange.Is(StronglyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(middle_Openness).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness)).And(
                Linguistic_Extraversion.Is(middle_Extraversion))).Then(CognitiveChange.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(low_Openness).And(
                Linguistic_Agreeableness.Is(low_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(low_Conscientiousness)).And(
                Linguistic_Extraversion.Is(low_Extraversion))).Then(CognitiveChange.Is(WeaklyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = fuzzyEngine.Defuzzify(new
            {
                conscientiousness = (double)conscientiousness,
                extraversion = (double)extraversion,
                neuroticism = (double)neuroticism,
                agreeableness = (double)agreeableness,
                openness = (double)openness
            });

            Dictionary<string, float> LinguisticResult = new Dictionary<string, float>()
            {
                { "Weakly", (float)WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", (float)LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", (float)StronglyApplied.Fuzzify(OutputDefuzzify) },
            };
            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            var StrategyResults = new KeyValuePair<string, string>(EmotionRegulationModel.COGNITIVE_CHANGE, StrategyPower);

            return StrategyResults;
        }

        ////Response modulation
        private KeyValuePair<string,string> FuzzyfieResponseModulation()
        {
            var ResponseModulation = new LinguisticVariable("ResponseModulation");
            var WeaklyApplied = ResponseModulation.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = ResponseModulation.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = ResponseModulation.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(high_Extraversion).And(
                Linguistic_Openness.Is(high_Openness)).And(
                Linguistic_Agreeableness.Is(high_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(high_Conscientiousness)).And(
                Linguistic_Neuroticism.Is(high_Neuroticism))).Then(ResponseModulation.Is(WeaklyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(middle_Extraversion).And(
                Linguistic_Openness.Is(middle_Openness)).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness)).And(
                Linguistic_Neuroticism.Is(middle_Neuroticism))).Then(ResponseModulation.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(low_Extraversion).And(
                Linguistic_Openness.Is(low_Openness)).And(
                Linguistic_Agreeableness.Is(low_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(low_Conscientiousness)).And(
                Linguistic_Neuroticism.Is(low_Neuroticism))).Then(ResponseModulation.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3);

            var OutputDefuzzify = fuzzyEngine.Defuzzify(new
            {
                conscientiousness = (double)conscientiousness,
                extraversion = (double)extraversion,
                neuroticism = (double)neuroticism,
                agreeableness = (double)agreeableness,
                openness = (double)openness
            });

            Dictionary<string, float> LinguisticResult = new Dictionary<string, float>()
            {
                { "Weakly", (float)WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", (float)LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", (float)StronglyApplied.Fuzzify(OutputDefuzzify) },
            };
            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            KeyValuePair<string, string> StrategyResults = new KeyValuePair<string, string>(EmotionRegulationModel.RESPONSE_MODULATION, StrategyPower);

            return StrategyResults;
        }
#endregion
    }
}
