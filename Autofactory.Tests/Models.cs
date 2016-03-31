using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Autofactory.Tests
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class LocaleAttribute : Attribute
    {
        public string Locale { get; set; }
    }

    public interface ILocale
    {
    }

    [Locale(Locale = "en-US")]
    public class Class1 : LocaleBase
    {

    }

    [Locale(Locale = "en-US")]
    [Locale(Locale = "ko-KR")]
    public class Class2 : LocaleBase
    {

    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    internal sealed class DescriptionAttribute : Attribute
    {
        public string Name { get; set; }
        public DescriptionAttribute(string name)
        {
            Name = name;
        }
    }

    [Locale(Locale = "Common")]
    public class LocaleBase : ILocale
    {

    }

    public interface IAnimal
    {
        int Age { get; set; }
        IAnimal Friend { get; set; }
    }

    public abstract class Animal : IAnimal
    {
        public static int Dogs = 0;
        public static int Cats = 0;
        public static int Ducks = 0;
        public static int CtorIAnimal = 0;
        public static int CtorAgeFriend = 0;
        public static int CtorFriendAge = 0;
        public static int Instances { get { return Dogs + Cats + Ducks; } }
        public int Age { get; set; }
        public IAnimal Friend { get; set; }
        public static void Reset()
        {
            Dogs = Cats = Ducks = CtorAgeFriend = CtorIAnimal = CtorFriendAge = 0;
        }
    }
    [Description("Dog")]
    public class Dog : Animal
    {
        public Dog()
        {
            Interlocked.Increment(ref Dogs);
        }

        public Dog(int age) : this()
        {
            Age = age;
        }

        public Dog(Animal friend) : this()
        {
            Friend = friend;
        }

        public Dog(IAnimal friend) : this()
        {
            Interlocked.Increment(ref CtorIAnimal);
            Friend = friend;
        }

        public Dog(int age, Animal friend) : this()
        {
            Interlocked.Increment(ref CtorAgeFriend);
            Age = age;
            Friend = friend;
        }

        public Dog(Animal friend, int age) : this()
        {
            Interlocked.Increment(ref CtorFriendAge);
            Age = age;
            Friend = friend;
        }
    }

    [Description("Cat")]
    public class Cat : Animal
    {
        public Cat()
        {
            Interlocked.Increment(ref Cats);
        }

        public Cat(int age) : this()
        {
            Age = age;
        }

        public Cat(Animal friend) : this()
        {
            Friend = friend;
        }

        public Cat(IAnimal friend) : this()
        {
            Interlocked.Increment(ref CtorIAnimal);
            Friend = friend;
        }

        public Cat(int age, Animal friend) : this()
        {
            Interlocked.Increment(ref CtorAgeFriend);
            Age = age;
            Friend = friend;
        }

        public Cat(Animal friend, int age) : this()
        {
            Interlocked.Increment(ref CtorFriendAge);
            Age = age;
            Friend = friend;
        }

    }

    public class Duck : Animal
    {
        public Duck()
        {
            Interlocked.Increment(ref Ducks);
        }

        public Duck(int age)
            : this()
        {
            Age = age;
        }

        public Duck(Animal friend)
            : this()
        {
            Friend = friend;
        }

        public Duck(IAnimal friend)
            : this()
        {
            Interlocked.Increment(ref CtorIAnimal);
            Friend = friend;
        }

        public Duck(int age, Animal friend)
            : this()
        {
            Interlocked.Increment(ref CtorAgeFriend);
            Age = age;
            Friend = friend;
        }

        public Duck(Animal friend, int age)
            : this()
        {
            Interlocked.Increment(ref CtorFriendAge);
            Age = age;
            Friend = friend;
        }
    }

    public class Lion : Animal
    {
    }
}
