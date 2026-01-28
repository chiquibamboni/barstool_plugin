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
        [Description("Установка MinValue больше MaxValue выбрасывает" +
            " исключение")]
        public void MinAboveMax_ThrowsException()
        {
            var parameter = new Parameter(50, 10, 100);

            string expectedMessage = "Минимальное значение (150) не" +
                " может быть больше максимального значения (100)";

            Assert.That(() => parameter.MinValue = 150,
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedMessage));
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
        public void MaxAboveMin_ThrowsException()
        {
            var parameter = new Parameter(50, 10, 100);

            string expectedMessage = "Максимальное значение (5) не может" +
                " быть меньше минимального значения (10)";

            Assert.That(() => parameter.MaxValue = 5,
                Throws.InstanceOf<ArgumentException>()
                    .With.Message.Contains(expectedMessage));
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