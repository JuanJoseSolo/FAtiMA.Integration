using System;
using System.Collections.Generic;
using System.Linq;
using EmotionRegulation.Components;
using RolePlayCharacter;
using WellFormedNames;
using ActionLibrary;
using EmotionalAppraisal;
using ActionLibrary.DTOs;
using EmotionalAppraisal.DTOs;
using System.Diagnostics;

namespace EmotionRegulation.BigFiveModel
{
    [Serializable]
    public class BaseAgent
    {
        public RolePlayCharacterAsset FAtiMACharacter { get; internal set; }
        //public Name AgentName { get; private set; }
        public string DominantPersonality { get; private set; }
        public List<KeyValuePair<string, string>> StrategyMetrics { get; private set; }
        public List<string> StrategiesToApply { get; private set; }
        public List<string> AllPersonalities { get; private set; }
        public RequiredData RequiredData { get; set; }
        public EmotionRegulationModel Results { get => results; set => results = value; }
        public Name AgentName { get => agentName;}

        internal PersonalityDTO personality;
        internal EmotionRegulationModel results;
        internal RolePlayCharacterAsset auxCharacter;
        private Name agentName;

        public BaseAgent(RolePlayCharacterAsset agentFAtiMA, PersonalityDTO personalityDTO, RequiredData info)
        {
            if (personalityDTO is null)
            {
                throw new ArgumentNullException(nameof(personalityDTO));
            }
            
            FAtiMACharacter = agentFAtiMA ?? throw new ArgumentNullException(nameof(agentFAtiMA));
            RequiredData = info ?? throw new ArgumentNullException(nameof(info));
            CreateAgente(personalityDTO);
        }

        private void CreateAgente(PersonalityDTO personalityDTO)
        {
            if(personalityDTO is null)
                throw new ArgumentNullException(nameof(personalityDTO));
            if (RequiredData is null)
                throw new ArgumentNullException(nameof(RequiredData));

            agentName = FAtiMACharacter.CharacterName;
            var personality = new Personality(personalityDTO);
            var BigFive = personality.BigFive;
            DominantPersonality = BigFive.DominantPersonality;
            StrategiesToApply = BigFive.StrategiesToApply;
            AllPersonalities = BigFive.AllPersonalities;
            StrategyMetrics = BigFive.StrategyMetrics;
            this.personality = personalityDTO;
            var copyAppraisalRules = FAtiMACharacter.m_emotionalAppraisalAsset.GetAllAppraisalRules();
            var auxEA = new EmotionalAppraisalAsset();
            foreach (var appRule in copyAppraisalRules)
                auxEA.AddOrUpdateAppraisalRule(appRule);
            auxCharacter = new RolePlayCharacterAsset() { m_emotionalAppraisalAsset = auxEA, CharacterName = agentName };
        }


        public IAction Regulate(IAction decision, string initiator = "*") // es importante conocer quién realiza la acción?
        {
            if (decision is null)
                return null;
            if (this is null)
                throw new ArgumentException("The new agent is null", nameof(BaseAgent));
            if (FAtiMACharacter is null)
                throw new ArgumentException("The character is null", nameof(FAtiMACharacter));

            ///-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            /// Paso 2.1:
            ///     Parte del segundo paso consiste en verificar si la posible decisión provocará una emoción negativa en el 
            ///     agente.La clase LoadingEmotionalRegulation contiene un método que cácula las posibles emociones y otro que 
            ///     verifica la existencia de datos suficientes(eventos que para ser evitados, acciones para la segunda 
            ///     estrategia, etc.) para llevar a cabo la regulación emocional. A través del método ApplyEmotionRegulation se 
            ///     revisa si se cumplen estas dos condiciones (que el evento provoque emociones negativas y que existan los 
            ///     datos necesarios). Si se cumplen las condiciones el método retorna un valor verdadero y se crea un nuevo 
            ///     objeto de tipo EmotionRegulationModel para aplicar las estrategias ahí definidas.
            ///-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

            var data = new LoadingEmotionalRegulation(); 
            
            var applyRegulation = data.ApplyEmotionRegulation(this, decision, initiator);
            
            if (applyRegulation) 
            {
                ///-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                /// Paso 3:
                ///     Una vez se han verificado todos los datos, se procede a relizar la regualción de la emoción(s) causada
                ///     por el evento. Se crea el objeto EmotionRegulationModel, pasando como parámetros la acción/evento/
                ///     decision que probocará la emoción(s) negativa y el objeto creado data, el cual contiene los datos 
                ///     necesarios para iniciar con la regulación.
                ///-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                
                Results = new EmotionRegulationModel(decision, data);
            
                return Results.newAction;
            }
            else
                return null;
        }

    }
}
