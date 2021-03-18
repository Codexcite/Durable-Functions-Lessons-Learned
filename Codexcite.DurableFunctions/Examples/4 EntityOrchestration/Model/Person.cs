using System;

namespace Codexcite.DurableFunctions.Examples.EntityOrchestration.Model
{
	public class Person
	{
		//public Person()
		//{
			//Id = Guid.NewGuid().ToString(); // Don't do this if you create the object in a Orchestrator! 
		//}

		public Person(string id, string? name = null, int? age = default, Gender? gender = default)
		{
			Name = name;
			Age = age;
			Gender = gender;
			Id = id;
		}

		public string Id { get; set; }
		public string? Name;
		public int? Age;
		public Gender? Gender;

		
	}
}