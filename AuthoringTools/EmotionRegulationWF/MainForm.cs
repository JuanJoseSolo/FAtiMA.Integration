using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using EmotionalAppraisal;
using EmotionalAppraisal.DTOs;
using EmotionalAppraisalWF.ViewModels;
using GAIPS.AssetEditorTools;
using GAIPS.Rage;
using Equin.ApplicationFramework;




namespace EmotionRegulationWF
{
    public partial class MainForm : Form
    {
        private AssetStorage _storage;
        private EmotionalAppraisalAsset _loadedAsset;
        private AppraisalRulesVM _appraisalRulesVM;
        private string _currentFilePath;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _storage = new AssetStorage();
            _loadedAsset = EmotionalAppraisalAsset.CreateInstance(_storage);
            OnAssetDataLoaded();
        }

        public EmotionalAppraisalAsset AssetForRegulation
        {
            get { return _loadedAsset; }
            set { _loadedAsset = value; OnAssetDataLoaded(); }
        }


        private void OnAssetDataLoaded()
        {
            //Appraisal Rule
            _appraisalRulesVM = new AppraisalRulesVM(AssetForRegulation);
            dataGridER.DataSource = _appraisalRulesVM.AppraisalRules.ToList();             
            EditorTools.HideColumns(dataGridER, new[]
            {
            PropertyUtil.GetPropertyName<AppraisalRuleDTO>(dto => dto.Id),
            PropertyUtil.GetPropertyName<AppraisalRuleDTO>(e => e.Conditions)
            });
            
            //conditionSetEditor.View = _appraisalRulesVM.CurrentRuleConditions;

            EditorTools.UpdateFormTitle("Emotional Appraisal", _currentFilePath, this);

        }






        private void buttonEditAppraisalRule_Click(object sender, EventArgs e)
        {

        }
    }
}
