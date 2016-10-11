#if NET451
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFactory;
using Xunit;

namespace Autofactory.Tests
{
    public class UnitTests_Net45
    {
        public UnitTests_Net45()
        {
            SetupForEachTest();
        }

        private void SetupForEachTest()
        {
            Animal.Reset();
        }

        [Fact]
        public void Test_Net45_MutipleAttribute()
        {
            var factory = AutoFactory.Factory.Create<ILocale>();
            var parts = factory.SeekPartsFromAttribute<LocaleAttribute>(attr => attr.Locale == "en-US").ToList();
            Assert.Equal(2, parts.Count);
            Assert.True(parts.Any(p => p.GetType() == typeof(Class1)));
            Assert.True(parts.Any(p => p.GetType() == typeof(Class2)));
            var part = factory.SeekPartFromAttribute<LocaleAttribute>(attr => attr.Locale == "ko-KR");
            Assert.True(part.GetType() == typeof(Class2));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var err = factory.SeekPartFromAttribute<LocaleAttribute>(attr => attr.Locale == "en-US");
            });
            var commonPart = factory.SeekPartFromAttribute<LocaleAttribute>(attr => attr.Locale == "Common");
            var allparts = factory.SeekPartsFromAttribute<LocaleAttribute>(attr => true).ToList();
            Assert.Equal(typeof(LocaleBase), commonPart.GetType());
            Assert.Equal(4, allparts.Count);
        }

        [Fact]
        public void Test_Net45_Lazyness()
        {
            var fact = AutoFactory.Factory.Create<IAnimal>();
            Assert.Equal(4, fact.GetPartTypes().Length);
            Assert.Equal(0, Animal.Instances);
            var dog = fact.GetPart<Dog>();
            Assert.Equal(1, Animal.Instances);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(0, Animal.Cats);
        }

        [Fact]
        public void Test_Net45_CreateOnlyOnce()
        {
            var fact = AutoFactory.Factory.Create<IAnimal>(typeof(Animal).Assembly);
            var dog = fact.GetPart<Dog>();
            var dog2 = fact.GetPart<Dog>();
            var dog3 = fact.GetPart<Dog>();
            Assert.Equal(1, Animal.Instances);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(0, Animal.Cats);
        }

        [Fact]
        public void Test_Net45_Create_CtorIntParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From<int>(20));
            var dog20 = fact.SeekPart(p => p.Name == "Dog");

            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(20, dog20.Age);
        }

        [Fact]
        public void Test_Net45_Create_CtorAnimalParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From<Animal>(new Cat(5)));
            var dog = fact.SeekPart(p => p.Name == "Dog");

            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(5, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(0, Animal.CtorIAnimal);
        }

        [Fact]
        public void Test_Net45_Create_CtorIAnimalParam()
        {
            var fact = AutoFactory.Factory.Create<Animal>(new[] { typeof(Animal).Assembly },
                TypedParameter.From<IAnimal>(new Cat(2)));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(1, Animal.CtorIAnimal);
        }

        [Fact]
        public void Test_Net45_Create_CtorTwoParams()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From<Animal>(new Cat(2)),
                TypedParameter.From(1));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(1, Animal.CtorFriendAge);
            Assert.Equal(0, Animal.CtorAgeFriend);
        }

        [Fact]
        public void Test_Net45_Create_CtorTwoParams_Rev()
        {
            var fact = AutoFactory.Factory.Create<Animal>(typeof(Animal).Assembly, TypedParameter.From(1),
                TypedParameter.From<Animal>(new Cat(2)));
            var dog = fact.SeekPart(p => p.Name == "Dog");
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, dog.Friend.Age);
            Assert.Equal("Cat", dog.Friend.GetType().Name);
            Assert.Equal(1, Animal.CtorAgeFriend);
            Assert.Equal(0, Animal.CtorFriendAge);
        }

        [Fact]
        public void Test_Net45_SeekPart()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(10));
            var cat = fact.SeekPart(t => t.Name == "Cat");
            Assert.Equal(1, Animal.Cats);
            Assert.Equal(10, cat.Age);
        }

        [Fact]
        public void Test_Net45_SeekPart_Exception()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(10));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var cat = fact.SeekPart(t => t.Name != "Cat");
            });
        }

        [Fact]
        public void Test_Net45_SeekParts()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(10));
            var noCats = fact.SeekParts(t => t.Name.StartsWith("D")).ToList();
            Assert.Equal(0, Animal.Cats);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(1, Animal.Ducks);
            Assert.Equal(2, noCats.Count);
            Assert.Equal(10, noCats.First().Age);
        }

        [Fact]
        public void Test_Net45_SeekPartFromAttr()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(7));
            var dog =
                fact.SeekPartFromAttribute<DescriptionAttribute>(_ => _.Name == "Dog");
            Assert.Equal(0, Animal.Cats);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(7, dog.Age);
        }

        [Fact]
        public void Test_Net45_SeekPartsFromAttr()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(7));
            var withAttr = fact.SeekPartsFromAttribute<DescriptionAttribute>(_ => true).ToList();
            Assert.Equal(1, Animal.Cats);
            Assert.Equal(1, Animal.Dogs);
            Assert.Equal(2, withAttr.Count);
        }

        [Fact]
        public void Test_Net45_ResolutionFail()
        {
            var fact = AutoFactory.Factory.Create<Animal>(TypedParameter.From(7));
            Assert.Throws<AutoFactoryException>(() =>
            {
                var part = fact.SeekPart(t => t.Name == "Lion");
            });
        }

        [Fact]
        public void Test_Net45_NonGeneric()
        {
            var fact = AutoFactory.Factory.Create(typeof(Animal));
            var duck = fact.GetPart(typeof(Duck));
            var dog = fact.SeekPart(t => t.Name == "Dog");
            var cat = fact.SeekPartFromAttribute<DescriptionAttribute>(a => a.Name == "Cat");
            Assert.IsType<Duck>(duck);
            Assert.IsType<Dog>(dog);
            Assert.IsType<Cat>(cat);
            Assert.Equal(3, Animal.Instances);
        }

    }
}
#endif