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
        [Description("Value устанавливается и возвращается в пределах" +
            " диапазона")]
        public void Value_SetAndGet_ShouldWorkForValidRange()
        {
            var parameter = new Parameter(50, 10, 100);
            parameter.Value = 64;
            Assert.That(parameter.Value, Is.EqualTo(64));
        }

        [Test]
        [Description("Value выбрасывает исключение при значении ниже" +
            " минимума")]
        public void Value_SetBelowMin_ShouldThrowArgumentOutOfRangeException()
        {
            var parameter = new Parameter(50, 10, 100);
            var expectedMessage = "Значение должно быть в " +
                "диапазоне [10, 100]";

            Assert.That(() => parameter.Value = 5,
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("Value выбрасывает исключение при значении" +
            " выше максимума")]
        public void Value_SetAboveMax_ShouldThrowArgumentOutOfRangeException()
        {
            var parameter = new Parameter(50, 10, 100);
            var expectedMessage = "Значение должно быть в" +
                " диапазоне [10, 100]";

            Assert.That(() => parameter.Value = 105,
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("Value может быть установлено равным минимальному" +
            " значению")]
        public void Value_SetToMinValue_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);
            parameter.Value = 10;
            Assert.That(parameter.Value, Is.EqualTo(10));
        }

        [Test]
        [Description("Value может быть установлено равным максимальному" +
            " значению")]
        public void Value_SetToMaxValue_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);
            parameter.Value = 100;
            Assert.That(parameter.Value, Is.EqualTo(100));
        }

        [Test]
        [Description("Конструктор выбрасывает исключение при значении ниже" +
            " минимума")]
        public void Constructor_WithDefaultBelowMin_ShouldThrowException()
        {
            var expectedMessage = "Значение должно быть в диапазоне [10, 100]";

            Assert.That(() => new Parameter(5, 10, 100),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("Конструктор выбрасывает исключение при значении" +
            " выше максимума")]
        public void Constructor_WithDefaultAboveMax_ShouldThrowException()
        {
            var expectedMessage = "Значение должно быть в " +
                "диапазоне [10, 100]";

            Assert.That(() => new Parameter(105, 10, 100),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedMessage));
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
        [Description("Установка MinValue выше текущего значения " +
            "выбрасывает исключение")]
        public void MinValue_SetAboveCurrentValue_ShouldThrowException()
        {
            var parameter = new Parameter(50, 10, 100);
            var expectedMessage =
                $"Текущее значение ({50}) меньше нового минимального" +
                $" значения ({60})";

            Assert.That(() => parameter.MinValue = 60,
                Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("Установка MaxValue ниже текущего значения " +
            "выбрасывает исключение")]
        public void MaxValue_SetBelowCurrentValue_ShouldThrowException()
        {
            var parameter = new Parameter(50, 10, 100);
            var expectedMessage =
                $"Текущее значение ({50}) больше нового максимального" +
                $" значения ({40})";

            Assert.That(() => parameter.MaxValue = 40,
                Throws.InstanceOf<InvalidOperationException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("IsValueInRange правильно проверяет значения" +
            " в диапазоне")]
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

        [Test]
        [Description("Установка MinValue больше MaxValue выбрасывает" +
            " исключение")]
        public void MinValue_SetGreaterThanMaxValue_ShouldThrowArgumentException()
        {
            var parameter = new Parameter(50, 10, 100);

            string expectedMessage = "Минимальное значение (150) не" +
                " может быть больше максимального значения (100)";

            Assert.That(() => parameter.MinValue = 150,
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("Установка MinValue равной MaxValue должна работать")]
        public void MinValue_SetEqualToMaxValue_ShouldWork()
        {
            var parameter = new Parameter(100, 10, 100);
            Assert.That(() => parameter.MinValue = 100, Throws.Nothing);
            Assert.That(parameter.MinValue, Is.EqualTo(100));
        }

        [Test]
        [Description("Установка MinValue меньше текущего MinValue" +
            " должна работать")]
        public void MinValue_SetBelowCurrentMin_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);
            Assert.That(() => parameter.MinValue = 5, Throws.Nothing);
            Assert.That(parameter.MinValue, Is.EqualTo(5));
        }

        [Test]
        [Description("Установка MaxValue меньше MinValue выбрасывает" +
            " исключение")]
        public void MaxValue_SetLessThanMinValue_ShouldThrowArgumentException()
        {
            var parameter = new Parameter(50, 10, 100);

            string expectedMessage = "Максимальное значение (5) не может" +
                " быть меньше минимального значения (10)";

            Assert.That(() => parameter.MaxValue = 5,
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedMessage));
        }

        [Test]
        [Description("Установка MaxValue равной MinValue должна работать")]
        public void MaxValue_SetEqualToMinValue_ShouldWork()
        {
            var parameter = new Parameter(10, 10, 100);
            Assert.That(() => parameter.MaxValue = 10, Throws.Nothing);
            Assert.That(parameter.MaxValue, Is.EqualTo(10));
        }

        [Test]
        [Description("Установка MaxValue больше текущего MaxValue" +
            " должна работать")]
        public void MaxValue_SetAboveCurrentMax_ShouldWork()
        {
            var parameter = new Parameter(50, 10, 100);
            Assert.That(() => parameter.MaxValue = 150, Throws.Nothing);
            Assert.That(parameter.MaxValue, Is.EqualTo(150));
        }
    }
}