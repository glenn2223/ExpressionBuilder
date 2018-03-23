using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using LambdaExpressionBuilder.Common;
using LambdaExpressionBuilder.Helpers;
using LambdaExpressionBuilder.Resources;
using LambdaExpressionBuilder.Interfaces;
using System.Collections.Generic;
using System.Collections;

namespace ExpressionBuilder.WinForms.Controls
{
	/// <summary>
	/// Description of UcFilter.
	/// </summary>
	public partial class UcFilter : UserControl
	{
		string _typeName = "ExpressionBuilder.Models.Person";
        IPropertyCollection _properties;
		
		[Category("Data")]
		public string TypeName
		{
			get { return _typeName ?? "ExpressionBuilder.WinForms.Models.Person"; }
			set { _typeName = value; }
		}
		
		public string PropertyId
		{
			get { return (cbProperties.SelectedItem as Property).Id; }
		}
		
		public Operation Operation
		{
			get { return (Operation)Enum.Parse(typeof(Operation), (cbOperations.SelectedItem as dynamic).Id); }
		}
		
		public object Value
		{
			get
			{
                var numberOfValues = new OperationHelper().NumberOfValuesAcceptable(Operation, MatchType);
                if (numberOfValues == -1) numberOfValues = 1;
                return numberOfValues > 0 ? GetValue("ctrlValue") : null;
			}
		}

        public MatchType MatchType
        {
            get
            {
                if (cbMatchTypes.Items.Count == 0 || cbMatchTypes.SelectedItem == null)
                    return MatchType.All;
                return (MatchType)Enum.Parse(typeof(MatchType), cbMatchTypes.SelectedItem.ToString());
            }
        }

        private object GetValue(string ctrlName)
        {
            var property = _properties[PropertyId];
            var type = Nullable.GetUnderlyingType(property.Info.PropertyType) ?? property.Info.PropertyType;
            var myList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            
            foreach (Control ctrl in valuesContainer.Controls)
            {
                if (ctrl.Name.StartsWith("ctrlValue"))
                {
                    if (type == typeof(string)) myList.Add(ctrl.Text);
                    if (type == typeof(DateTime)) myList.Add((ctrl as DateTimePicker).Value);
                    if (type == typeof(int)) myList.Add(Convert.ToInt32((ctrl as NumericUpDown).Value));
                    if (type == typeof(bool)) myList.Add(Boolean.Parse(ctrl.Text));
                    if (type.IsEnum) myList.Add(Enum.ToObject(property.Info.PropertyType, (ctrl as DomainUpDown).SelectedItem));
                }
            }

            if (myList.Count > 0)
                if (myList.Count == 1)
                    return myList[0];
                else
                    return myList;

			return null;
        }

        public override bool ValidateChildren()
        {
            var isValid = base.ValidateChildren();

            if (String.IsNullOrWhiteSpace(cbOperations.Text))
            {
                MessageBox.Show("Please, select an operation");
                isValid = false;
            }

            return isValid;
        }

        public Connector Conector
		{
			get { return (Connector)Enum.Parse(typeof(Connector), cbConector.Text); }
		}
		
		public UcFilter()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		public event EventHandler OnAdd;
		public event EventHandler OnRemove;
		public event EventHandler OnAddGroup;
		
		void UcFilterLoad(object sender, EventArgs e)
		{
			LoadProperties();
            LoadOperations();
			cbProperties.SelectedIndex = 0;
			cbConector.SelectedIndex = 0;
		}
		
		public void LoadProperties()
		{
			var type = Type.GetType(TypeName, true);
			LoadProperties(type);
            cbProperties.ValueMember = "Id";
            cbProperties.DisplayMember = "Name";
			cbProperties.DataSource = _properties.ToList();
			cbProperties.SelectedIndexChanged += cbProperties_SelectedIndexChanged;
		}

        private void LoadOperations()
        {
            var type = _properties[PropertyId].Info.PropertyType;
            var supportedOperations = new OperationHelper()
                                        .SupportedOperations(type)
                                        .Select(o => new {
                                            Id = o.ToString(),
                                            Name = o.GetDescription(Resources.Operations.ResourceManager)
                                        })
                                        .ToArray();

            cbOperations.ValueMember = "Id";
            cbOperations.DisplayMember = "Name";
            cbOperations.DataSource = supportedOperations;

            LoadValueControls();
        }

		void cbProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
            LoadOperations();
        }
		
		void LoadValueControls()
		{
            LoadMatchTypes();

            var numberOfValues = new OperationHelper().NumberOfValuesAcceptable(Operation, MatchType);

            if (numberOfValues == -1) numberOfValues = 1;

            valuesContainer.Controls.Clear();

            for (var i = 0; i < numberOfValues; i++)
            {
                var ctrl = CreateNewControl();
                ctrl.Name = "ctrlValue" + (i + 1);
                ctrl.KeyDown += (s, e) => OnEnterAddControl(s, e, valuesContainer.Controls);
                valuesContainer.Controls.Add(ctrl);
            }
        }

        void LoadMatchTypes()
        {
            cbMatchTypes.Visible = true;
            cbMatchTypes.DataSource = null;

            var allowedMatchTypes = new OperationHelper().AllowedMatchTypes(Operation);
            if (allowedMatchTypes.Count == 0)
                cbMatchTypes.Visible = false;
            else
                cbMatchTypes.DataSource = allowedMatchTypes.ToArray();
        }

        Control CreateNewControl()
        {
            var info = _properties[PropertyId].Info;
            var underlyingNullableType = Nullable.GetUnderlyingType(info.PropertyType);
            var type = underlyingNullableType ?? info.PropertyType;
            Control ctrl = null;
            if (type.IsEnum || type == typeof(bool))
            {
                ctrl = new DomainUpDown();
                if (type == typeof(bool))
                {
                    (ctrl as DomainUpDown).Items.AddRange(new[] { true, false });
                }
                else
                {
                    (ctrl as DomainUpDown).Items.AddRange(Enum.GetValues(type));
                }
                (ctrl as DomainUpDown).SelectedItem = (ctrl as DomainUpDown).Items[0];
                (ctrl as DomainUpDown).ReadOnly = true;

            }

            if (type == typeof(string))
            {
                ctrl = new TextBox();
            }

            if (type == typeof(DateTime))
            {
                ctrl = new DateTimePicker();
            }

            if (new[] { typeof(int), typeof(double), typeof(float), typeof(decimal) }.Contains(type))
            {
                ctrl = new NumericUpDown();
                (ctrl as NumericUpDown).Value = 0;
            }

            if (ctrl == null) throw new Exception("Type not supported");

            return ctrl;
        }

        public IPropertyCollection LoadProperties(Type type)
		{
            return _properties = new PropertyCollection(type, Resources.Person.ResourceManager);

        }
		
		void BtnAddClick(object sender, EventArgs e)
		{
            OnAdd(sender, e);
		}
		
		void BtnRemoveClick(object sender, EventArgs e)
		{
			OnRemove(sender, e);
		}

        private void cbOperations_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadValueControls();
        }

        private void SetControlVisibility(string controlName, bool visible)
        {
            var ctrl = Controls.Find(controlName, false).FirstOrDefault();
            
            if (ctrl != null)
            {
                ctrl.Visible = visible;
            }
        }

        private void OnEnterAddControl(object sender, KeyEventArgs e, ControlCollection collection)
        {
            if (e.KeyCode != Keys.Enter || new OperationHelper().NumberOfValuesAcceptable(Operation, MatchType) != -1)
                return;

            var controlcount = Controls.Count;

            var ctrl = CreateNewControl();
            ctrl.Name = "ctrlValue" + controlcount;
            ctrl.KeyDown += (s, eA) => OnEnterAddControl(s, eA, collection);
            collection.Add(ctrl);
            ctrl.Focus();
        }

        private void MiAddGroup_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var strip = item.Owner as ContextMenuStrip;

            OnAddGroup((strip.SourceControl as Button).Parent.Parent, e);
        }
    }
}
