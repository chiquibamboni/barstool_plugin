using BarstoolPluginCore.Model;
using NUnit.Framework;

namespace BarstoolPluginTests
{
    [TestFixture]
    [Description("Тесты для класса Parameter")]
    public class ParameterTests
    {
        [Test]
        [Description("Конструктор корректно инициализирует все поля")]
        public void Constructor_ShouldInitializeAllFields()
        {
            var parameter = new Parameter(50, 10, 100);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.MinValue, Is.EqualTo(10));
                Assert.That(parameter.MaxValue, Is.EqualTo(100));
                Assert.That(parameter.Value, Is.EqualTo(50));
            });
        }

        [Test]
        [Description("Value корректно устанавливается и возвращается в " +
            "пределах диапазона")]
        public void Value_SetAndGet_ShouldWorkForValidRange()
        {
            var parameter = new Parameter(50, 10, 100);

            parameter.Value = 64;

            Assert.That(parameter.Value, Is.EqualTo(64));
        }

        [Test]
        [Description("Value выбрасывает исключение при установке значения " +
            "ниже минимума")]
        public void Value_SetBelowMin_ShouldThrowArgumentOutOfRangeException()
        {
            var parameter = new Parameter(50, 10, 100);

            Assert.That(() => parameter.Value = 5,
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(
                        "Значение должно быть в диапазоне [10, 100]"));
        }

        [Test]
        [Description("Value выбрасывает исключение при установке значения " +
            "выше максимума")]
        public void Value_SetAboveMax_ShouldThrowArgumentOutOfRangeException()
        {
            var parameter = new Parameter(50, 10, 100);

            Assert.That(() => parameter.Value = 105,
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(
                        "Значение должно быть в диапазоне [10, 100]"));
        }

        [Test]
        [Description("Value может быть установлено равным минимальному " +
            "значению")]
        public void Value_SetToMinValue_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);

            parameter.Value = 10;

            Assert.That(parameter.Value, Is.EqualTo(10));
        }

        [Test]
        [Description("Value может быть установлено равным максимальному " +
            "значению")]
        public void Value_SetToMaxValue_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);

            parameter.Value = 100;

            Assert.That(parameter.Value, Is.EqualTo(100));
        }

        [Test]
        [Description("Конструктор выбрасывает исключение если значение по " +
            "умолчанию ниже минимума")]
        public void Constructor_WithDefaultBelowMin_ShouldThrowException()
        {
            Assert.That(() => new Parameter(5, 10, 100),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(
                        "Значение должно быть в диапазоне [10, 100]"));
        }

        [Test]
        [Description("Конструктор выбрасывает исключение если значение по " +
            "умолчанию выше максимума")]
        public void Constructor_WithDefaultAboveMax_ShouldThrowException()
        {
            Assert.That(() => new Parameter(105, 10, 100),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(
                        "Значение должно быть в диапазоне [10, 100]"));
        }

        [Test]
        [Description("MinValue корректно устанавливается и возвращается")]
        public void MinValue_SetAndGet_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);

            parameter.MinValue = 20;

            Assert.That(parameter.MinValue, Is.EqualTo(20));
        }

        [Test]
        [Description("Установка MinValue выше текущего значения не меняет " +
            "Value")]
        public void MinValue_SetAboveCurrentValue_ShouldNotChangeValue()
        {
            var parameter = new Parameter(50, 10, 100);
            int originalValue = parameter.Value;

            parameter.MinValue = 60;

            Assert.Multiple(() =>
            {
                Assert.That(parameter.MinValue, Is.EqualTo(60));
                Assert.That(parameter.Value, Is.EqualTo(originalValue));
            });
        }

        [Test]
        [Description("MaxValue корректно устанавливается и возвращается")]
        public void MaxValue_SetAndGet_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);

            parameter.MaxValue = 400;

            Assert.That(parameter.MaxValue, Is.EqualTo(400));
        }

        [Test]
        [Description("Установка MaxValue ниже текущего значения не меняет " +
            "Value")]
        public void MaxValue_SetBelowCurrentValue_ShouldNotChangeValue()
        {
            var parameter = new Parameter(50, 10, 100);
            int originalValue = parameter.Value;

            parameter.MaxValue = 40;

            Assert.Multiple(() =>
            {
                Assert.That(parameter.MaxValue, Is.EqualTo(40));
                Assert.That(parameter.Value, Is.EqualTo(originalValue));
            });
        }

        [Test]
        [Description("IsValueInRange правильно проверяет значения в " +
            "диапазоне")]
        public void IsValueInRange_ShouldCorrectlyValidateValues()
        {
            var parameter = new Parameter(50, 10, 100);

            Assert.Multiple(() =>
            {
                Assert.That(() => parameter.Value = 10, Throws.Nothing);
                Assert.That(() => parameter.Value = 50, Throws.Nothing);
                Assert.That(() => parameter.Value = 100, Throws.Nothing);

                Assert.That(() => parameter.Value = 9,
                    Throws.InstanceOf<ArgumentOutOfRangeException>());
                Assert.That(() => parameter.Value = 101,
                    Throws.InstanceOf<ArgumentOutOfRangeException>());
            });
        }
    }
}