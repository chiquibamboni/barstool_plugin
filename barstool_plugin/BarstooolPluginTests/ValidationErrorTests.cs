using NUnit.Framework;
using BarstoolPluginCore.Model;
using System;
using System.Collections.Generic;

namespace BarstoolPluginTests
{
    [TestFixture]
    [Description("Тесты для класса ValidationError")]
    public class ValidationErrorTests
    {
        [Test]
        [Description(
            "Конструктор должен корректно инициализировать все поля")]
        public void Constructor_ShouldInitializeAllFields()
        {
            var affectedParameters = new List<ParameterType>
            {
                ParameterType.LegDiameterD1
            };
            var expectedMessage = "Значение выходит за допустимые пределы";
            var expectedFieldName = "Диаметр ножки";

            var error = new ValidationError(
                affectedParameters,
                expectedMessage,
                expectedFieldName);

            Assert.Multiple(() =>
            {
                Assert.That(error.AffectedParameters,
                    Is.EqualTo(affectedParameters));
                Assert.That(error.Message, Is.EqualTo(expectedMessage));
                Assert.That(error.FieldName, Is.EqualTo(expectedFieldName));
            });
        }

        [Test]
        [Description(
            "Свойство AffectedParameters должно возвращать копию списка")]
        public void AffectedParameters_ShouldReturnCopyOfList()
        {
            var originalList = new List<ParameterType>
            {
                ParameterType.LegDiameterD1
            };
            var error = new ValidationError(originalList, "Тест");

            originalList.Add(ParameterType.SeatDiameterD);

            Assert.That(error.AffectedParameters, Has.Count.EqualTo(1));
            Assert.That(error.AffectedParameters,
                Does.Not.Contain(ParameterType.SeatDiameterD));
        }

        [Test]
        [Description(
            "Свойство Message должно возвращать корректное значение")]
        public void Message_ShouldReturnCorrectValue()
        {
            var expectedMessage =
                "Диаметр ножки должен быть меньше диаметра сидения";
            var error = new ValidationError(
                new List<ParameterType> { ParameterType.LegDiameterD1 },
                expectedMessage);

            var actualMessage = error.Message;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        [Description(
            "Свойство FieldName должно возвращать корректное значение")]
        public void FieldName_ShouldReturnCorrectValue()
        {
            var expectedFieldName = "Высота стула";
            var error = new ValidationError(
                new List<ParameterType> { ParameterType.StoolHeightH },
                "Ошибка высоты",
                expectedFieldName);

            var actualFieldName = error.FieldName;

            Assert.That(actualFieldName, Is.EqualTo(expectedFieldName));
        }

        [Test]
        [Description(
            "Конструктор должен выбрасывать исключение при пустом "
            + "списке параметров")]
        public void Constructor_ShouldThrowExceptionForEmptyParameterList()
        {
            var emptyList = new List<ParameterType>();
            var message = "Тестовое сообщение";
            var expectedError = "Список параметров не может быть пустым";

            Assert.That(() => new ValidationError(emptyList, message),
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedError));
        }

        [Test]
        [Description(
            "Конструктор должен выбрасывать исключение при null "
            + "списке параметров")]
        public void Constructor_ShouldThrowExceptionForNullParameterList()
        {
            List<ParameterType> nullList = null;
            var message = "Тестовое сообщение";
            var expectedError = "Список параметров не может быть пустым";

            Assert.That(() => new ValidationError(nullList, message),
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedError));
        }

        [Test]
        [Description(
            "Конструктор должен выбрасывать исключение при пустом "
            + "сообщении")]
        public void Constructor_ShouldThrowExceptionForEmptyMessage()
        {
            var affectedParameters = new List<ParameterType>
            {
                ParameterType.LegDiameterD1
            };
            var expectedError = "Сообщение об ошибке не может быть пустым";

            Assert.That(() => new ValidationError(affectedParameters, ""),
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedError));
            Assert.That(() => new ValidationError(affectedParameters, "   "),
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedError));
            Assert.That(() => new ValidationError(affectedParameters, null),
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedError));
        }

        [Test]
        [Description("Можно создать ошибки для всех типов параметров")]
        public void Constructor_ShouldWorkForAllParameterTypes()
        {
            var allParameterTypes = Enum.GetValues(typeof(ParameterType));

            foreach (ParameterType parameterType in allParameterTypes)
            {
                var affectedParams = new List<ParameterType> { parameterType };
                var error = new ValidationError(affectedParams,
                    $"Error for {parameterType}");

                Assert.Multiple(() =>
                {
                    Assert.That(error.AffectedParameters,
                        Contains.Item(parameterType));
                    Assert.That(error.AffectedParameters,
                        Has.Count.EqualTo(1));
                    Assert.That(error.Message,
                        Is.EqualTo($"Error for {parameterType}"));
                });
            }
        }

        [Test]
        [Description(
            "Две ошибки с разными параметрами должны корректно работать")]
        public void TwoErrors_WithDifferentParameters_ShouldWorkCorrectly()
        {
            var error1 = new ValidationError(
                new List<ParameterType> { ParameterType.LegDiameterD1 },
                "Same message");

            var error2 = new ValidationError(
                new List<ParameterType> { ParameterType.SeatDiameterD },
                "Same message");

            Assert.Multiple(() =>
            {
                Assert.That(error1.AffectedParameters,
                    Does.Not.EqualTo(error2.AffectedParameters));
                Assert.That(error1.Message, Is.EqualTo(error2.Message));
            });
        }

        [Test]
        [Description("Свойства должны быть доступны только для чтения")]
        public void Properties_ShouldBeReadOnly()
        {
            var error = new ValidationError(
                new List<ParameterType> { ParameterType.SeatDepthS },
                "Test message",
                "Test field");

            Assert.Multiple(() =>
            {
                Assert.That(error.AffectedParameters, Is.Not.Null);
                Assert.That(error.Message, Is.Not.Null);
                Assert.That(() => error.FieldName, Throws.Nothing);

                var affectedParams = error.AffectedParameters;

                Assert.That(affectedParams,
                    Is.InstanceOf<List<ParameterType>>());
            });
        }
    }
}
