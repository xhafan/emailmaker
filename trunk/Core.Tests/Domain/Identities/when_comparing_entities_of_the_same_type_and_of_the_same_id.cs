﻿using NUnit.Framework;
using Shouldly;

namespace Core.Tests.Domain.Identities
{
    [TestFixture]
    public class when_comparing_entities_of_the_same_type_and_of_the_same_id
    {
        private Entity _entityOne;
        private Entity _entityTwo;

        [SetUp]
        public void Context()
        {
            _entityOne = new Entity(11);
            _entityTwo = new Entity(11);
        }

        [Test]
        public void entity_one_equals_entity_two()
        {
            _entityOne.Equals(_entityTwo).ShouldBe(true);
            (_entityOne.GetHashCode() == _entityTwo.GetHashCode()).ShouldBe(true);
        }

        [Test]
        public void entity_one_equals_entity_two_by_operator()
        {
            (_entityOne == _entityTwo).ShouldBe(true);
        }

        [Test]
        public void entity_one_equals_entity_two_by_negate_operator()
        {
            (_entityOne != _entityTwo).ShouldBe(false);
        }

    }
}