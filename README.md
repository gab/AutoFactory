# AutoFactory
A factory creator for strategies using AutoFac. Never write a Factory again!.

- Loose coupling between components -> Maintainability.
- No need to modify the factory when coding a new part -> Extensibility. 
- Can be used with existing parts without modifying its code -> Code simplicity.

Convert this:
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


