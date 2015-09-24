# AutoFactory
A factory creator for strategies using AutoFac. Never write a Factory again!.

### NuGet
```
PM> Install-Package AutoFactory
```

It's common to have a a Factory of Strategies pattern that selects one specific implementation of a base class (or interface) based on some convention on the concrete classes (i.e. an Attribute, the Class Name).

With AutoFactory you can convert this:
```
ISort GetSort(string algorithm) 
{
   switch(algorithm) 
   {
       case "QuickSort":
          return new QuickSort();
       case "MergeSort":
          return new MergeSort();
       case "BubbleSort":
          return new BubbleSort();
       ...
   }
}
```
Into this:
```
private IAutoFactory<ISort> factory = Factory.Create<ISort>();

ISort GetSort(string algorithm)
{
    return factory.SeekPart(t => t.Name.Equals(algorithm));
}
```

AutoFactory internally uses AutoFac to create a factory of concrete clases (parts) from a base class allowing to seek parts from any characteristic of its type before instantiating it (i.e. name convention, attribute convention).

Attribute Convention
=====

Suppose you have a strategy in which each class defines its behavior with an Attribute on the class:

    internal sealed class SortAlgorithmAttribute : Attribute
    {
        public string Name { get; set; }
        public SortAlgorithmAttribute(string name) { Name = name; }
    }

    public interface ISort { }

    [SortAlgorithm("Quick Sort")]
    public class QuickSort : ISort {}

    [SortAlgorithm("Merge Sort")]
    public class MergeSort : ISort {}

    [SortAlgorithm("Bubble Sort")]
    public class BubbleSort : ISort {}

In this case, the factory can use the `SeekPartFromAttribute` method:

    private IAutoFactory<ISort> factory = Factory.Create<ISort>();
    
    ISort GetSort(string algorithmName)
    {
        return factory.SeekPartFromAttribute<SortAlgorithmAttribute>(a => a.Name.Equals(algorithmName));
    }

Features
=====
- Parts can be seek by type convention or by attribute convention. 
- Parts are created only when the client requests it (lazy instantiation). 
- Supports constructor parameters injection.

Pros
=====
- Loose coupling between components -> Maintainability.
- No need to modify the factory when coding a new part -> Extensibility. 
- Can be used with existing parts without modifying its code -> Code simplicity.

Constraints
=====

The concrete classes (parts) must follow these rules:
- Parts must inherit from a common base class and/or implement a common interface.
- Parts must share a public constructor with or without parameters.
- Parts must not be generic classes.

