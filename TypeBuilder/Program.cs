

using System.Text.Json;
using TypeBuilderDemo;

var aggregate = new Customer(Guid.NewGuid(), "John", new Address("123 Main St", "Anytown"));
var flattener = new ObjectFlattener();
var obj = flattener.Flatten(aggregate);

foreach (var field in obj.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
{
	System.Console.WriteLine($"{field.Name}: {field.GetValue(obj)}");
}