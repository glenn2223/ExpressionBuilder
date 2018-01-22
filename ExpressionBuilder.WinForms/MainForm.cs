using ExpressionBuilder.WinForms.Controls;
using ExpressionBuilder.WinForms.Models;
using LambdaExpressionBuilder.Generics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ExpressionBuilder.WinForms
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
	{
		List<Person> _people;
		
		public List<Person> People
		{
			get
			{
                var company = new Person.Company { Name = "Back to the future", Industry = "Time Traveling Agency" };

                _people = new List<Person>();
                _people.Add(new Person { Name = "John Doe", Gender = PersonGender.Male, Birth = new Person.BirthData { Date = new DateTime(1979, 2, 28), Country = "USA" }, Employer = company });
                _people.Add(new Person { Name = "Jane Doe", Gender = PersonGender.Female, Birth = new Person.BirthData { Date = new DateTime(1985, 9, 5), Country = " " } });
                _people.Add(new Person { Name = "Wade Wilson", Gender = PersonGender.Male, Birth = new Person.BirthData { Date = new DateTime(1973, 10, 9), Country = "USA" } });
                _people.Add(new Person { Name = "Jessica Jones", Gender = PersonGender.Female, Birth = new Person.BirthData { Date = new DateTime(1980, 12, 20), Country = "USA" } });
                _people.Add(new Person { Name = "Jane Jones", Gender = PersonGender.Female, Birth = new Person.BirthData { Date = new DateTime(1980, 12, 20), Country = "USA" } });
                _people.Add(new Person { Name = "Fulano Silva", Gender = PersonGender.Male, Birth = new Person.BirthData { Date = new DateTime(1983, 5, 10), Country = "BRA" }, Employer = company });
                _people.Add(new Person { Name = "John Hancock", Gender = PersonGender.Male, Employer = company });

                var id = 1;
				foreach (var person in _people)
				{
					person.Id = id++;
					var email = person.Name.ToLower().Replace(" ", ".") + "@email.com";
					person.Contacts.Add(new Contact{ Type = ContactType.Email, Value = email, Comments = person.Name + "'s email" });
				}
				
				return _people;
			}
		}
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			AddFilter();
            ToggleIfLastButton(pnFilters, false);
            
            grid.DataSource = People;
        }

        protected void AddFilter()
		{
			AddFilter(pnFilters);
        }

        internal void AddGroup(Control control)
        {
            var group = new UcGroup();
            group.Name = "group" + control.Controls.Count;
            group.OnRemove += UcFilterOnRemoveGroup;

            control.Controls.Add(group);

            group.Size = new Size(control.Size.Width - 10, group.Height);
            group.Anchor = (AnchorStyles.Left | AnchorStyles.Right);

            AddFilter(group.groupOfFilters);
            ToggleIfLastButton(group.groupOfFilters, false);
        }

        internal void AddFilter(Control control)
        {
            var filter = new UcFilter();
            filter.TypeName = "ExpressionBuilder.WinForms.Models.Person";
            filter.OnAdd += UcFilterOnAdd;
            filter.OnRemove += UcFilterOnRemove;
            filter.OnAddGroup += UcFilterOnAddGroup;
            filter.Name = "filter" + control.Controls.Count;

            control.Controls.Add(filter);
            filter.Size = new Size(control.Size.Width - 10, filter.Height);
            filter.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
        }

        void UcFilterOnAdd(object sender, EventArgs e)
        {
            var control = (sender as Control).Parent.Parent;

            ToggleIfLastButton(control, true);

            AddFilter(control);
		}
		
		void UcFilterOnRemove(object sender, EventArgs e)
		{
			Control
                filter = (sender as Button).Parent,
                parent = filter.Parent;
			if (parent.Controls.Count > 1)
			{
				parent.Controls.Remove(filter);
			}

            ToggleIfLastButton(parent, false);
        }

        void UcFilterOnAddGroup(object sender, EventArgs e)
        {
            var control = sender as Control;
            ToggleIfLastButton(control, true);

            AddGroup(control);
        }

        void UcFilterOnRemoveGroup(object sender, EventArgs e)
        {
            var control = (sender as Button).Parent as UcGroup;
            if (control != null)
            {
                var parent = control.Parent;

                if (control.groupOfFilters.Controls.Count > 1)
                {
                    if (MessageBox.Show("Are you sure?", "Please confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        parent.Controls.Remove(control);
                    return;
                }

                parent.Controls.Remove(control);

                ToggleIfLastButton(parent, false);
            }
        }

        void ToggleIfLastButton(Control control, bool Visible)
        {
			if (control.Controls.Count == 1)
			{
				var childControl = control.Controls[0];
                if (childControl.GetType() == typeof(UcFilter))
                    (childControl as UcFilter).btnRemove.Visible = Visible;
                else if (childControl.GetType() == typeof(UcGroup))
                    (childControl as UcGroup).btnRemove.Visible = Visible;
            }
        }
		
		void ExecuteFilterF5ToolStripMenuItemClick(object sender, EventArgs e)
		{
			var filter = new Filter<Person>();

            GetFilters(ref filter, pnFilters);

            grid.DataSource = People.Where(filter).ToList();
		}

        void GetFilters(ref Filter<Person> filter, Control parentControl)
        {
            foreach (var control in parentControl.Controls)
            {
                if (control.GetType() == typeof(UcFilter))
                {
                    var ufilter = (UcFilter)control;
                    if (!ufilter.ValidateChildren())
                        break;

                    filter.By(ufilter.PropertyId, ufilter.Operation, ufilter.Value, ufilter.Conector, ufilter.MatchType);
                }
                else if (control.GetType() == typeof(UcGroup))
                {
                    var uGroup = (UcGroup)control;
                    if (uGroup.groupOfFilters.Controls.Count > 0)
                    {
                        filter.StartGroup();
                        GetFilters(ref filter, uGroup.groupOfFilters);
                        filter.EndGroup();
                    }
                }
            }
        }
    }
}