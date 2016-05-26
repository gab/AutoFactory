#if NETCOREAPP1_0 
using System;
using System.Linq;
using System.Reflection;
using AutoFactory;
using Xunit;

namespace Autofactory.Tests
{
    public class UnitTests_CoreDnx
    {
        public static Assembly ThisAssembly = typeof (UnitTests_CoreDnx).GetTypeInfo().Assembly;

        public UnitTests_CoreDnx()
        {
            SetupForEachTest();
        }

        private void SetupForEachTest()
        {
            Animal.Reset();
        }

        [Fact]
        public void Test_DnxCore_MutipleAttribute()
        {
            var factory = AutoFactory.Factory.Create<ILocale>(ThisAssembly);
            var parts = factory.SeekPartsFromAttribute<LocaleAttribute>(attr => attr.Locale == "en-US").ToList();
            Assert.Equal(2, parts.Count);
            Assert.True(parts.Any(p => p.GetType() == typeof (Class1)));
            Assert.True(parts.Any(p => p.GetType() == typeof (Class2)));
            var part = factory.SeekPartFromAttribute<LocaleAttribute>(attr => attr.Locale == "ko-KR");
            Assert.True(part.GetType() == typeof (Class2));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var err = factory.SeekPartFromAttribute<LocaleAttribute>(attr => attr.Locale == "en-US");
            });
        }

        [Fact]
        public void Test_DnxCore_Lazyness()
        {
            var fact = AutoFactory.Factory.Create<IAnimal>(ThisAssembly);
            Assert.Equal(4, fact.GetPartTypes().Length);
            Assert.Equal(0, Animal.Instances);
            var dog = fact.GetPart<Dog>();
            Assert.Equal(1, Animal.Instances);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(0, Animal.Cats);
        }

        [Fact]
        public void Test_DnxCore_CreateOnlyOnce()
        {
            var fact = AutoFactory.Factory.Create<IAnimal>(typeof(Animal).GetTypeInfo().Assembly);
            var dog = fact.GetPart<Dog>();
            var dog2 = fact.GetPart<Dog>();
            var dog3 = fact.GetPart<Dog>();
            Assert.Equal(1, Animal.Instances);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(0, Animal.Cats);
        }

        [Fact]
        public void Test_DnxCore_Create_CtorIntParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From<int>(20));
            var dog20 = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(20, dog20.Age);
        }

        [Fact]
        public void Test_DnxCore_Create_CtorAnimalParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From<Animal>(new Cat(5)));
            var dog = fact.SeekPart(p => p.Name == "Dog");

            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(5, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(0, Animal.CtorIAnimal);
        }

        [Fact]
        public void Test_DnxCore_Create_CtorIAnimalParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(new[] { typeof(Animal).GetTypeInfo().Assembly }, TypedParameter.From<IAnimal>(new Cat(2)));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(1, Animal.CtorIAnimal);
        }

        [Fact]
        public void Test_DnxCore_Create_CtorTwoParams()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From<Animal>(new Cat(2)), TypedParameter.From(1));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(1, Animal.CtorFriendAge);
            Assert.Equal(0, Animal.CtorAgeFriend);
        }

        [Fact]
        public void Test_DnxCore_Create_CtorTwoParams_Rev()
        {
            var fact = AutoFactory.Factory.Create<Animal>(typeof(Animal).GetTypeInfo().Assembly, TypedParameter.From(1), TypedParameter.From<Animal>(new Cat(2)));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(1, Animal.CtorAgeFriend);
            Assert.Equal(0, Animal.CtorFriendAge);
        }

        [Fact]
        public void Test_DnxCore_SeekPart()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From(10));
            var cat = fact.SeekPart(t => t.Name == "Cat");
            Assert.Equal(1, Animal.Cats);
            Assert.Equal(10, cat.Age);
        }

        [Fact]
        public void Test_DnxCore_SeekPart_Exception()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From(10));
            Assert.Throws<AutoFactoryException>(() =>
            {
                var cat = fact.SeekPart(t => t.Name != "Cat");
            });
        }

        [Fact]
        public void Test_DnxCore_SeekParts()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From(10));
            var noCats = fact.SeekParts(t => t.Name.StartsWith("D")).ToList();
            Assert.Equal(0, Animal.Cats);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(1, Animal.Ducks);
            Assert.Equal(2, noCats.Count);
            Assert.Equal(10, noCats.First().Age);
        }

        [Fact]
        public void Test_DnxCore_SeekPartFromAttr()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From(7));
            var dog = fact.SeekPartFromAttribute<DescriptionAttribute>(_ => _.Name == "Dog");
            Assert.Equal(0, Animal.Cats);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(7, dog.Age);
        }

        [Fact]
        public void Test_DnxCore_SeekPartsFromAttr()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From(7));
            var withAttr = fact.SeekPartsFromAttribute<DescriptionAttribute>(_ => true).ToList();
            Assert.Equal(1, Animal.Cats);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, withAttr.Count);
        }

        [Fact]
        public void Test_DnxCore_ResolutionFail()
        {
            var fact = AutoFactory.Factory.Create<Animal>(ThisAssembly, TypedParameter.From(7));
            Assert.Throws<AutoFactoryException>(() =>
            {
                var part = fact.SeekPart(t => t.Name == "Lion");
            });
        }

        [Fact]
        public void Test_DnxCore_NonGeneric()
        {
            var fact = AutoFactory.Factory.Create(typeof(Animal));
            var duck = fact.GetPart(typeof(Duck));
            var dog = fact.SeekPart(t => t.Name == "Dog");
            var cat = fact.SeekPartFromAttribute<DescriptionAttribute>(a => a.Name == "Cat");
            Assert.IsType(typeof(Duck), duck);
            Assert.IsType(typeof(Dog), dog);
            Assert.IsType(typeof(Cat), cat);
            Assert.Equal(3, Animal.Instances);
        }

    }
}
#endif