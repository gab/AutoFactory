using System.Linq;
using System.Threading;
using AutoFactory;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class UnitTests
    {
        [SetUp]
        public void SetupForEachTest()
        {
            Animal.Reset();
        }

        [Test]
        public void Test_Lazyness()
        {
            var fact = AutoFactory.Factory.Create<IAnimal>();
            Assert.AreEqual(4, fact.GetPartTypes().Length);
            Assert.AreEqual(0, Animal.Instances);
            var dog = fact.GetPart<Dog>();
            Assert.AreEqual(1, Animal.Instances);
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(0, Animal.Cats);
        }

        [Test]
        public void Test_CreateOnlyOnce()
        {
            var fact = AutoFactory.Factory.Create<IAnimal>(typeof(Animal).Assembly);
            var dog = fact.GetPart<Dog>();
            var dog2 = fact.GetPart<Dog>();
            var dog3 = fact.GetPart<Dog>();
            Assert.AreEqual(1, Animal.Instances);
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(0, Animal.Cats);
        }

        [Test]
        public void Test_Create_CtorIntParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From<int>(20));
            var dog20 = fact.SeekPart(p => p.Name == "Dog");

            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(20, dog20.Age);
        }

        [Test]
        public void Test_Create_CtorAnimalParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From<Animal>(new Cat(5)));
            var dog = fact.SeekPart(p => p.Name == "Dog");

            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(5, dog.Friend.Age);
            Assert.AreEqual("Cat", dog.Friend.GetType().Name);
            Assert.AreEqual(0, Animal.CtorIAnimal);
        }

        [Test]
        public void Test_Create_CtorIAnimalParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(new []{ typeof(Animal).Assembly }, TypedParameter.From<IAnimal>(new Cat(2)));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(2, dog.Friend.Age);
            Assert.AreEqual("Cat", dog.Friend.GetType().Name);
            Assert.AreEqual(1, Animal.CtorIAnimal);
        }

        [Test]
        public void Test_Create_CtorTwoParams()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From<Animal>(new Cat(2)), TypedParameter.From(1));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(2, dog.Friend.Age);
            Assert.AreEqual("Cat", dog.Friend.GetType().Name);
            Assert.AreEqual(1, Animal.CtorFriendAge);
            Assert.AreEqual(0, Animal.CtorAgeFriend);
        }

        [Test]
        public void Test_Create_CtorTwoParams_Rev()
        {
            var fact = AutoFactory.Factory.Create<Animal>(typeof(Animal).Assembly, TypedParameter.From(1), TypedParameter.From<Animal>(new Cat(2)));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(2, dog.Friend.Age);
            Assert.AreEqual("Cat", dog.Friend.GetType().Name);
            Assert.AreEqual(1, Animal.CtorAgeFriend);
            Assert.AreEqual(0, Animal.CtorFriendAge);
        }

        [Test]
        public void Test_SeekPart()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(10));
            var cat = fact.SeekPart(t => t.Name == "Cat");
            Assert.AreEqual(1, Animal.Cats);
            Assert.AreEqual(10, cat.Age);
        }

        [Test]
        public void Test_SeekPart_Exception()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(10));
            Assert.Throws<AutoFactoryException>(() =>
            {
                var cat = fact.SeekPart(t => t.Name != "Cat");
            });
        }

        [Test]
        public void Test_SeekParts()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(10));
            var noCats = fact.SeekParts(t => t.Name.StartsWith("D")).ToList();
            Assert.AreEqual(0, Animal.Cats);
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(1, Animal.Ducks);
            Assert.AreEqual(2, noCats.Count);
            Assert.AreEqual(10, noCats.First().Age);
        }

        [Test]
        public void Test_SeekPartFromAttr()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(7));
            var dog = fact.SeekPartFromAttribute<DescriptionAttribute>(_ => _.Properties["Description"][0].ToString() == "Dog");
            Assert.AreEqual(0, Animal.Cats);
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(7, dog.Age);
        }

        [Test]
        public void Test_SeekPartsFromAttr()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(7));
            var withAttr = fact.SeekPartsFromAttribute<DescriptionAttribute>(_ => true).ToList();
            Assert.AreEqual(1, Animal.Cats);
            Assert.AreEqual(1, Animal.Dogs);
            Assert.AreEqual(2, withAttr.Count);
        }

        [Test]
        public void Test_ResolutionFail()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(7));
            Assert.Throws<AutoFactoryException>(() =>
            {
                var part = fact.SeekPart(t => t.Name == "Pete");
            });
        }

        [Test]
        public void Test_NonGeneric()
        {
            var fact = AutoFactory.Factory.Create(typeof(Animal));
            var duck = fact.GetPart(typeof(Duck));
            var dog = fact.SeekPart(t => t.Name == "Dog");
            var cat = fact.SeekPartFromAttribute<DescriptionAttribute>(a => a.Properties["Description"][0].ToString() == "Cat");
            Assert.IsInstanceOf<Duck>(duck);
            Assert.IsInstanceOf<Dog>(dog);
            Assert.IsInstanceOf<Cat>(cat);
            Assert.AreEqual(3, Animal.Instances);
        }

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

    public class Pete : Animal
    {
    }
}
