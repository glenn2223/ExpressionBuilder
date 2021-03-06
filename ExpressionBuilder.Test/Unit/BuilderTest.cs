﻿using System;
using System.Collections.Generic;
using System.Linq;
using LambdaExpressionBuilder.Common;
using LambdaExpressionBuilder.Exceptions;
using LambdaExpressionBuilder.Generics;
using ExpressionBuilder.Test.Models;
using NUnit.Framework;

namespace ExpressionBuilder.Test.Unit
{
	[TestFixture]
	public class BuilderTest
	{
		readonly List<Person> _people;
		
		public List<Person> People
		{
			get
			{
				return _people;
			}
		}

        public BuilderTest()
        {
            var company = new Person.Company { Name = "Back to the future", Industry = "Time Traveling Agency" };

            _people = new List<Person>
            {
                new Person { Name = "John Doe", Gender = PersonGender.Male, Salary=4565, Birth = new Person.BirthData { Date = new DateTime(1979, 2, 28), Country = "USA" }, Employer = company },
                new Person { Name = "Jane Doe", Gender = PersonGender.Female, Salary=4973, Birth = new Person.BirthData { Date = new DateTime(1985, 9, 5), Country = " " } },
                new Person { Name = "Wade Wilson", Gender = PersonGender.Male, Salary=3579, Birth = new Person.BirthData { Date = new DateTime(1973, 10, 9), Country = "USA" } },
                new Person { Name = "Jessica Jones", Gender = PersonGender.Female, Salary=5000, Birth = new Person.BirthData { Date = new DateTime(1980, 12, 20), Country = "USA" } },
                new Person { Name = "Jane Jones", Gender = PersonGender.Female, Salary=3500, Birth = new Person.BirthData { Date = new DateTime(1980, 12, 20), Country = "AUS" } },
                new Person { Name = "Fulano Silva", Gender = PersonGender.Male, Salary=3322, Birth = new Person.BirthData { Date = new DateTime(1983, 5, 10), Country = "BRA" }, Employer = company },
                new Person { Name = "John Hancock", Gender = PersonGender.Male, Employer = company }
            };
            var id = 1;
            foreach (var person in _people)
            {
                person.Id = id++;

                if (id <= 5)
                {
                    var email = person.Name.ToLower().Replace(" ", ".") + "@email.com";
                    person.Contacts.Add(new Contact { Type = ContactType.Email, Value = email, Comments = person.Name + "'s email" });
                }
            }
        }
		
		[TestCase(TestName="Build expression from an empty filter: should return all records")]
        public void BuilderWithEmptyFilter()
        {
        	var filter = new Filter<Person>();
            var people = People.Where(filter);
            var solution = People;
            Assert.That(people, Is.EquivalentTo(solution));
        }
        
        [TestCase(TestName="Build expression from a filter with simple statements")]
        public void BuilderWithSimpleFilterStatements()
        {
        	var filter = new Filter<Person>();
            filter.By("Name", Operation.EndsWith, "Doe").Or.By("Gender", Operation.EqualTo, PersonGender.Female);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Name.Trim().ToLower().EndsWith("doe") ||
                                             p.Gender == PersonGender.Female);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Build expression from a filter casting the value to object")]
        public void BuilderCastingTheValueToObject()
        {
            var filter = new Filter<Person>();
            filter.By("Id", Operation.GreaterThan, (object)2);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Id > 2);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName="Build expression from a filter with property chain filter statements")]
        public void BuilderWithPropertyChainFilterStatements()
        {
        	var filter = new Filter<Person>();
        	filter.By("Birth.Country", Operation.EqualTo, "usa", Connector.Or);
        	filter.By("Birth.Date", Operation.LessThanOrEqualTo, new DateTime(1980, 1, 1), connector: Connector.Or);
        	filter.By("Name", Operation.Contains, "Doe");
            var people = People.Where(filter);
            var solution = People.Where(p => (p.Birth != null && p.Birth.Country != null && p.Birth.Country.Trim().ToLower().Equals("usa")) ||
                                             (p.Birth != null && p.Birth.Date <= new DateTime(1980, 1, 1)) ||
                                             p.Name.Trim().ToLower().Contains("doe"));
            Assert.That(people, Is.EquivalentTo(solution));
        }
        
        [TestCase(TestName="Build expression from a filter with property list filter statements")]
        public void BuilderWithPropertyListFilterStatements()
        {
        	var filter = new Filter<Person>();
        	filter.By("Contacts[Type]", Operation.EqualTo, ContactType.Email).And.By("Birth.Country", Operation.StartsWith, " usa ");
            var people = People.Where(filter);
            var solution = People.Where(p => p.Contacts.Any(c => c.Type == ContactType.Email) &&
                                             (p.Birth != null && p.Birth.Country.Trim().ToLower().StartsWith("usa")));
            Assert.That(people, Is.EquivalentTo(solution));
        }
        
        [TestCase(TestName="Build expression from a filter statement with a list of values")]
        public void BuilderWithFilterStatementWithListOfValues()
        {
        	var filter = new Filter<Person>();
        	filter.By("Id", Operation.EqualTo, new []{ 1, 2, 4, 5 }, matchType: MatchType.Any);
            var people = People.Where(filter);
            var solution = People.Where(p => new[] { 1, 2, 4, 5 }.Contains(p.Id));
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName="Builder with a single filter statement using a between operation")]
        public void BuilderWithSingleFilterStatementWithBetween()
        {
            var filter = new Filter<Person>();
            filter.By("Id", Operation.Between, new[] { 2, 4 });
            var people = People.Where(filter);
            var solution = People.Where(p => p.Id >= 2 && p.Id <= 4);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName="Builder with a single filter statement using a between operation and a simple statement")]
        public void BuilderWithBetweenAndSimpleFilterStatements()
        {
            var filter = new Filter<Person>();
            filter.By("Id", Operation.Between,new[] { 2, 6 }).And.By("Birth.Country", Operation.EqualTo, " usa ");
            var people = People.Where(filter);
            var solution = People.Where(p => (p.Id >= 2 && p.Id <= 6) &&
                                             (p.Birth != null && p.Birth.Country.Trim().ToLower().StartsWith("usa")));
            Assert.That(people, Is.EquivalentTo(solution));
            Assert.That(people.All(p => p.Birth.Country == "USA"), Is.True);
        }

        [TestCase(TestName = "Builder with a single filter statement using a between operation and a list of values statement")]
        public void BuilderWithBetweenAndListOfValuesFilterStatements()
        {
            var filter = new Filter<Person>();
            filter.By("Id", Operation.Between, new[] { 2, 6 }).And.By("Id", Operation.EqualTo, new[] { 4, 5 }, matchType: MatchType.Any);
            var people = People.Where(filter);
            var solution = People.Where(p => (p.Id >= 2 && p.Id <= 6) &&
                                             new[] { 4, 5 }.Contains(p.Id));
            Assert.That(people, Is.EquivalentTo(solution));
            Assert.That(people.Min(p => p.Id), Is.EqualTo(4));
        }

        [TestCase(TestName = "Builder using 'IsNull' operator")]
        public void BuilderUsingIsNullOperation()
        {
            var filter = new Filter<Person>();
            filter.By("Employer", Operation.IsNull);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Employer == null);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsNull' operator on an inner property")]
        public void BuilderUsingIsNullOperationOnAnInnerProperty()
        {
            var filter = new Filter<Person>();
            filter.By("Employer.Name", Operation.IsNull);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Employer == null || (p.Employer != null && p.Employer.Name == null));
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsNotNull' operator")]
        public void BuilderUsingIsNotNullOperation()
        {
            var filter = new Filter<Person>();
            filter.By("Employer", Operation.IsNotNull);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Employer != null);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsNotNull' operator on an inner property")]
        public void BuilderUsingIsNotNullOperationOnAnInnerProperty()
        {
            var filter = new Filter<Person>();
            filter.By("Employer.Name", Operation.IsNotNull);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Employer != null && p.Employer.Name != null);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsEmpty' operator")]
        public void BuilderUsingIsEmptyOperation()
        {
            var filter = new Filter<Person>();
            filter.By("Birth.Country", Operation.IsEmpty, (object)null, Connector.And);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Birth != null && p.Birth.Country != null && p.Birth.Country.Trim() == string.Empty);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsNotEmpty' operator")]
        public void BuilderUsingIsNotEmptyOperation()
        {
            var filter = new Filter<Person>();
            filter.By("Birth.Country", Operation.IsNotEmpty);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Birth != null && p.Birth.Country != null && p.Birth.Country.Trim() != string.Empty);
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsNullOrWhiteSpace' operator")]
        public void BuilderUsingIsNullOrWhiteSpaceOperation()
        {
            var filter = new Filter<Person>();
            filter.By("Birth.Country", Operation.IsNullOrWhiteSpace);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Birth != null && string.IsNullOrWhiteSpace(p.Birth.Country));
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using 'IsNotNullNorWhiteSpace' operator")]
        public void BuilderUsingIsNotNullNorWhiteSpaceOperation()
        {
            var filter = new Filter<Person>();
            filter.By("Birth.Country", Operation.IsNotNullNorWhiteSpace);
            var people = People.Where(filter);
            var solution = People.Where(p => p.Birth != null && !string.IsNullOrWhiteSpace(p.Birth.Country));
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder with wrong number of values when expecting no values at all")]
        public void BuilderWithWrongNumberOrValuesWhenExpectingNoValuesAtAll()
        {
            var filter = new Filter<Person>();
            var ex = Assert.Throws<WrongNumberOfValuesException>(() => filter.By("Id", Operation.IsNull, new[] { 1, 2 }));
            Assert.That(ex.Message, Does.Match(@"The operation '\w*' admits exactly '\w*' values \(not more neither less than this\)."));
        }

        [TestCase(TestName = "Builder with wrong number of values when expecting just one value")]
        public void BuilderWithWrongNumberOrValuesWhenExpectingJustOneValue()
        {
            var filter = new Filter<Person>();
            var ex = Assert.Throws<WrongNumberOfValuesException>(() => filter.By("Id", Operation.EqualTo, new[] { 1, 2 }));
            Assert.That(ex.Message, Does.Match(@"The operation '\w*' admits exactly '\w*' values \(not more neither less than this\)."));
        }

        [TestCase(TestName = "Builder with operation not supported by specific type")]
        public void BuilderWithOperationNotSupportedBySpecificType()
        {
            var filter = new Filter<Person>();
            var ex = Assert.Throws<UnsupportedOperationException>(() => filter.By("Name", Operation.GreaterThan, "John"));
            Assert.That(ex.Message, Does.Match(@"The type '\w*' does not have support for the operation '\w*'."));
        }

        [TestCase(TestName = "Builder working with nullable values")]
        public void BuilderWithNullableValues()
        {
            var filter = new Filter<Person>();
            filter.By("Birth.Date", Operation.IsNotNull)
                  .Or.By("Birth.Date", Operation.GreaterThan, new DateTime(1980, 1, 1));
            var people = People.Where(filter);
            var solution = People.Where(p => (p.Birth != null && p.Birth.Date != null)
                                            || (p.Birth != null && p.Birth.Date.HasValue && p.Birth.Date > new DateTime(1980, 1, 1)));
            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder working with custom supported type")]
        public void BuilderUsingCustomSupportedType()
        {
            var dateOffset = new DateTimeOffset(new DateTime(1980, 1, 1));
            var filter = new Filter<Person>();
            filter.By("Birth.DateOffset", Operation.GreaterThan, dateOffset);
            var people = People.Where(filter);
            var solution = People.Where(p => (p.Birth != null && p.Birth.DateOffset.HasValue && p.Birth.DateOffset > dateOffset));
            Assert.That(people, Is.EquivalentTo(solution));
        }

        private IQueryable<Person> GetPeople()
        {
            var filter = new Filter<Person>();
            filter.By("Employer.Name", Operation.IsNotNull);
            return People.AsQueryable().Where(filter);
        }

        [TestCase(TestName = "Builder using IQueryable")]
        public void BuilderUsingIQueryable()
        {
            var people = GetPeople();
            Assert.That(people, Is.InstanceOf<IQueryable<Person>>());

            var solution = People.AsQueryable().Where(p => p.Employer != null && p.Employer.Name != null).OrderByDescending(p => p.Name).Take(1);
            var person = (people as IQueryable<Person>).OrderByDescending(p => p.Name).Take(1);
            Assert.That(person.Count(), Is.EqualTo(1));
            Assert.That(person, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using complex expressions (fluent interface)", Category = "ComplexExpressions")]
        public void BuilderUsingComplexExpressionsFluentInterface()
        {
            var filter = new Filter<Person>();
            filter
                .OpenGroup
                    .OpenGroup
                        .By("Name", Operation.DoesNotContain, "doe")
                        .Or
                        .OpenGroup
                            .By("Name", Operation.EndsWith, "Doe")
                            .Or
                            .By("Name", Operation.StartsWith, "Jo")
                        .CloseGroup
                    .CloseGroup
                    .And
                    .By("Employer", Operation.IsNull)
                .CloseGroup
                .And
                .By("Birth.Country", Operation.EqualTo, "USA");
            var people = People.Where(filter);

            var solution = People.Where(x =>
                (
                    (
                        x.Name != null && !x.Name.Trim().ToLower().Contains("doe".Trim().ToLower())
                        ||
                        (
                            x.Name != null && x.Name.Trim().ToLower().EndsWith("Doe".Trim().ToLower())
                            ||
                            (x.Birth == null || x.Birth != null && (x.Birth.Country == null || x.Birth.Country.Trim() == ""))
                        )
                    )
                    &&
                    x.Employer == null
                )
                &&
                x.Birth != null && x.Birth.Country != null && x.Birth.Country.Trim().ToLower() == "USA".Trim().ToLower()
            );

            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder using complex expressions", Category = "ComplexExpressions")]
        public void BuilderUsingComplexExpressions()
        {
            var filter = new Filter<Person>();
            filter.StartGroup();
            filter.StartGroup();
            filter.By("Name", Operation.DoesNotContain, "doe", connector: Connector.Or);
            filter.StartGroup();
            filter.By("Name", Operation.EndsWith, "Doe", connector: Connector.Or);
            filter.By("Name", Operation.StartsWith, "Jo", connector: Connector.And);
            filter.EndGroup();
            filter.EndGroup();
            filter.By("Employer", Operation.IsNull, Connector.And);
            filter.EndGroup();
            filter.By("Birth.Country", Operation.EqualTo, "USA");
            var people = People.Where(filter);

            var solution = People.Where(x =>
                (
                    (
                        x.Name != null && !x.Name.Trim().ToLower().Contains("doe".Trim().ToLower())
                        ||
                        (
                            x.Name != null && x.Name.Trim().ToLower().EndsWith("Doe".Trim().ToLower())
                            ||
                            (x.Birth == null || x.Birth != null && (x.Birth.Country == null || x.Birth.Country.Trim() == ""))
                        )
                    )
                    &&
                    x.Employer == null
                )
                &&
                x.Birth != null && x.Birth.Country != null && x.Birth.Country.Trim().ToLower() == "USA".Trim().ToLower()
            );

            Assert.That(people, Is.EquivalentTo(solution));
        }

        [TestCase(TestName = "Builder not using complex expressions (different results)", Category = "ComplexExpressions")]
	    public void BuilderNotUsingComplexExpressions()
	    {
	        var filter = new Filter<Person>();
	        filter.By("Salary", Operation.GreaterThanOrEqualTo, 4000D).And.By("Salary", Operation.LessThanOrEqualTo, 5000D)
	            .And
                .By("Birth.Country", Operation.EqualTo, "USA").Or.By("Birth.Country", Operation.EqualTo, "AUS");
	        var people = People.Where(filter);
	        var solution = People.Where(p => ((p.Birth != null && p.Birth.Country == "USA") || (p.Birth != null && p.Birth.Country == "AUS"))
	                                         && (p.Salary >= 4000 && p.Salary <= 5000));

	        Assert.That(people, Is.Not.EquivalentTo(solution));
        }
    }
}