using NUnit.Framework;
using BarstoolPluginCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarstoolPluginTests
{
    [TestFixture]
    [Description("Тесты для класса Parameters")]
    public class ParametersTests
    {
        #region Existing Tests

        [Test]
        [Description(
            "Конструктор должен инициализировать значения по умолчанию")]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            var parameters = new Parameters();

            Assert.Multiple(() =>
            {
                Assert.That(parameters.GetValue(
                    ParameterType.LegDiameterD1), Is.EqualTo(50));
                Assert.That(parameters.GetValue(
                    ParameterType.FootrestDiameterD2), Is.EqualTo(30));
                Assert.That(parameters.GetValue(
                    ParameterType.SeatDiameterD), Is.EqualTo(400));
                Assert.That(parameters.GetValue(
                    ParameterType.FootrestHeightH1), Is.EqualTo(300));
                Assert.That(parameters.GetValue(
                    ParameterType.StoolHeightH), Is.EqualTo(800));
                Assert.That(parameters.GetValue(
                    ParameterType.SeatDepthS), Is.EqualTo(60));
                Assert.That(parameters.GetValue(
                    ParameterType.LegCountC), Is.EqualTo(4));
            });
        }

        [Test]
        [Description(
            "GetValue должен возвращать корректное значение параметра")]
        public void GetValue_ShouldReturnCorrectValue()
        {
            var parameters = new Parameters();

            Assert.Multiple(() =>
            {
                Assert.That(parameters.GetValue(
                    ParameterType.LegDiameterD1), Is.EqualTo(50));
                Assert.That(parameters.GetValue(
                    ParameterType.SeatDiameterD), Is.EqualTo(400));
            });
        }

        [Test]
        [Description("GetMin должен возвращать корректное значение")]
        public void GetMin_ShouldReturnCorrectValue()
        {
            var parameters = new Parameters();

            Assert.Multiple(() =>
            {
                Assert.That(parameters.GetMin(
                    ParameterType.LegDiameterD1), Is.EqualTo(25));
                Assert.That(parameters.GetMin(
                    ParameterType.SeatDiameterD), Is.EqualTo(300));
            });
        }

        [Test]
        [Description("GetMax должен возвращать корректное значение")]
        public void GetMax_ShouldReturnCorrectValue()
        {
            var parameters = new Parameters();

            Assert.Multiple(() =>
            {
                Assert.That(parameters.GetMax(
                    ParameterType.LegDiameterD1), Is.EqualTo(70));
                Assert.That(parameters.GetMax(
                    ParameterType.SeatDiameterD), Is.EqualTo(500));
            });
        }

        [Test]
        [Description(
            "SetValue должен устанавливать значение в допустимом диапазоне")]
        public void SetValue_ShouldSetValueWithinValidRange()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.SeatDiameterD, 400);

            int newValue = 50;
            bool result = parameters.SetValue(
                ParameterType.LegDiameterD1, newValue);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(parameters.GetValue(
                    ParameterType.LegDiameterD1), Is.EqualTo(newValue));
                Assert.That(parameters.HasErrors, Is.False);
            });
        }

        [Test]
        [Description(
            "SetValue должен возвращать false при выходе за "
            + "минимальную границу")]
        public void SetValue_ShouldReturnFalseForMinBoundaryViolation()
        {
            var parameters = new Parameters();
            int invalidValue = 24;
            bool result = parameters.SetValue(
                ParameterType.LegDiameterD1, invalidValue);
            var expectedError =
                "Значение 24 не находится в диапазоне [25, 70]";

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(parameters.HasErrors, Is.True);
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain(expectedError));
            });
        }

        [Test]
        [Description(
            "SetValue должен возвращать false при выходе за "
            + "максимальную границу")]
        public void SetValue_ShouldReturnFalseForMaxBoundaryViolation()
        {
            var parameters = new Parameters();
            int invalidValue = 71;
            bool result = parameters.SetValue(
                ParameterType.LegDiameterD1, invalidValue);
            var expectedError =
                "Значение 71 не находится в диапазоне [25, 70]";

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(parameters.HasErrors, Is.True);
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain(expectedError));
            });
        }

        [Test]
        [Description(
            "ValidateDependencies должен находить ошибку при "
            + "LegDiameter >= SeatDiameter/6")]
        public void ValidateDependencies_ShouldDetectLegDiameterRatioError()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.LegDiameterD1, 60);
            parameters.SetValue(ParameterType.SeatDiameterD, 300);
            parameters.SetValue(ParameterType.StoolHeightH, 750);

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.True);
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain("d1 < D/6"));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.LegDiameterD1));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.SeatDiameterD));
            });
        }

        [Test]
        [Description(
            "ValidateDependencies должен находить ошибку при "
            + "FootrestDiameter > LegDiameter")]
        public void ValidateDependencies_ShouldDetectFootrestDiameterError()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.FootrestDiameterD2, 30);
            parameters.SetValue(ParameterType.LegDiameterD1, 25);
            parameters.SetValue(ParameterType.StoolHeightH, 750);
            var expectedError = "не может быть больше диаметра ножки";

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.True);
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain(expectedError));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.LegDiameterD1));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.FootrestDiameterD2));
            });
        }

        [Test]
        [Description(
            "ValidateDependencies должен находить ошибку при "
            + "SeatDepth >= 3 * LegDiameter")]
        public void ValidateDependencies_ShouldDetectSeatDepthError()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.SeatDepthS, 75);
            parameters.SetValue(ParameterType.LegDiameterD1, 25);
            parameters.SetValue(ParameterType.StoolHeightH, 750);

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.True);
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain("S < 3*d1"));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.LegDiameterD1));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.SeatDepthS));
            });
        }

        [Test]
        [Description(
            "ValidateDependencies не должен находить ошибок при "
            + "валидных параметрах")]
        public void ValidateDependencies_ShouldNotFindErrorsWithValidParams()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.LegDiameterD1, 40);
            parameters.SetValue(ParameterType.SeatDiameterD, 400);
            parameters.SetValue(ParameterType.FootrestDiameterD2, 35);
            parameters.SetValue(ParameterType.SeatDepthS, 100);

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.False);
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Is.Empty);
                Assert.That(parameters.GetErrorMessages(), Is.Empty);
            });
        }

        [Test]
        [Description(
            "RemoveErrorsForParameter должен очищать ошибки для параметра")]
        public void RemoveErrorsForParameter_ShouldClearErrorsForParameter()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.LegDiameterD1, 24);

            Assert.That(parameters.HasErrors, Is.True);
            Assert.That(parameters.GetAllParametersWithErrors(),
                Does.Contain(ParameterType.LegDiameterD1));

            parameters.RemoveErrorsForParameter(ParameterType.LegDiameterD1);

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.False);
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Not.Contain(ParameterType.LegDiameterD1));
            });
        }

        [Test]
        [Description("AddFormError должен добавлять ошибку формы")]
        public void AddFormError_ShouldAddFormError()
        {
            var parameters = new Parameters();
            var affectedParameters = new List<ParameterType>
            {
                ParameterType.LegDiameterD1,
                ParameterType.SeatDiameterD
            };
            string errorMessage = "Кастомная ошибка формы";
            parameters.AddFormError(
                affectedParameters, errorMessage, "Поле1");

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.True);
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain(errorMessage));
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain("Поле1"));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.LegDiameterD1));
                Assert.That(parameters.GetAllParametersWithErrors(),
                    Does.Contain(ParameterType.SeatDiameterD));
            });
        }

        [Test]
        [Description(
            "GetAllParametersWithErrors должен возвращать "
            + "уникальные типы параметров")]
        public void GetAllParametersWithErrors_ShouldReturnUniqueTypes()
        {
            var parameters = new Parameters();
            var affectedParams = new List<ParameterType>
            {
                ParameterType.LegDiameterD1,
                ParameterType.SeatDiameterD,
                ParameterType.SeatDepthS
            };
            parameters.AddFormError(
                affectedParams, "Комплексная ошибка валидации");

            var errorParameters = parameters.GetAllParametersWithErrors();

            Assert.Multiple(() =>
            {
                Assert.That(errorParameters, Has.Count.EqualTo(3));
                Assert.That(errorParameters.Distinct().Count(),
                    Is.EqualTo(3));
                Assert.That(errorParameters,
                    Does.Contain(ParameterType.LegDiameterD1));
                Assert.That(errorParameters,
                    Does.Contain(ParameterType.SeatDiameterD));
                Assert.That(errorParameters,
                    Does.Contain(ParameterType.SeatDepthS));
            });
        }

        [Test]
        [Description(
            "GetErrorMessages должен возвращать "
            + "форматированные сообщения")]
        public void GetErrorMessages_ShouldReturnFormattedErrorMessages()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.LegDiameterD1, 24);
            parameters.AddFormError(
                new List<ParameterType> { ParameterType.SeatDiameterD },
                "Ошибка валидации",
                "Диаметр сидения");

            string errorMessages = parameters.GetErrorMessages();
            var expectedRangeError =
                "Значение 24 не находится в диапазоне [25, 70]";
            var expectedFormatError =
                "Диаметр сидения: Ошибка валидации";

            Assert.Multiple(() =>
            {
                Assert.That(errorMessages, Does.Contain(expectedRangeError));
                Assert.That(errorMessages, Does.Contain(expectedFormatError));
                Assert.That(errorMessages, Does.Contain("\n"));
            });
        }

        [Test]
        [Description(
            "HasErrors должен правильно отражать наличие ошибок")]
        public void HasErrors_ShouldCorrectlyIndicateErrorPresence()
        {
            var parameters = new Parameters();
            Assert.That(parameters.HasErrors, Is.False);

            parameters.SetValue(ParameterType.LegDiameterD1, 24);
            Assert.That(parameters.HasErrors, Is.True);

            parameters.RemoveErrorsForParameter(ParameterType.LegDiameterD1);
            Assert.That(parameters.HasErrors, Is.False);
        }

        [Test]
        [Description(
            "SetValue с fieldName должен включать имя поля в ошибку")]
        public void SetValue_WithFieldName_ShouldIncludeFieldNameInError()
        {
            var parameters = new Parameters();
            string fieldName = "Диаметр ножки";
            bool result = parameters.SetValue(
                ParameterType.LegDiameterD1, 24, fieldName);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(
                    parameters.GetErrorMessages(), Does.Contain(fieldName));
            });
        }

        #endregion

        #region Added Tests for 100% Coverage

        [Test]
        [Description(
            "ValidateDependencies должен находить ошибку пересечения ножек")]
        public void ValidateDependencies_ShouldDetectLegIntersectionError()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.SeatDepthS, 90);
            parameters.SetValue(ParameterType.LegCountC, 6);
            parameters.SetValue(ParameterType.LegDiameterD1, 70);
            parameters.SetValue(ParameterType.SeatDiameterD, 310);

            var assertMsg = "Сообщение об ошибке должно содержать текст "
                    + "о пересечении ножек";

            Assert.Multiple(() =>
            {
                Assert.That(parameters.HasErrors, Is.True,
                    "Должна быть ошибка");
                Assert.That(parameters.GetErrorMessages(),
                    Does.Contain("Ножки стула пересекаются"), assertMsg);

                var errorParams = parameters.GetAllParametersWithErrors();
                Assert.That(errorParams,
                    Does.Contain(ParameterType.LegDiameterD1));
                Assert.That(errorParams,
                    Does.Contain(ParameterType.SeatDiameterD));
                Assert.That(errorParams,
                    Does.Contain(ParameterType.LegCountC));
                Assert.That(errorParams,
                    Does.Contain(ParameterType.SeatDepthS));
            });
        }

        [Test]
        [Description(
            "RemoveErrorsForParameter должен очищать ошибки зависимостей")]
        public void RemoveErrorsForParameter_ShouldClearDependencyErrors()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.FootrestDiameterD2, 50);
            parameters.SetValue(ParameterType.LegDiameterD1, 40);
            Assert.That(parameters.HasErrors, Is.True,
                "Изначально должна быть ошибка");

            parameters.RemoveErrorsForParameter(ParameterType.LegDiameterD1);

            Assert.That(parameters.HasErrors, Is.False,
                "Ошибка должна быть удалена");
        }

        [Test]
        [Description(
            "AddFormError должен очищать предыдущие ошибки "
            + "для тех же параметров")]
        public void AddFormError_ShouldClearPreviousErrorsForSameParams()
        {
            var parameters = new Parameters();
            parameters.SetValue(
                ParameterType.LegDiameterD1, 10, "ДиаметрНожки");
            string initialError = parameters.GetErrorMessages();
            Assert.That(initialError,
                Does.Contain("не находится в диапазоне"));

            var affected = new List<ParameterType>
                { ParameterType.LegDiameterD1 };
            parameters.AddFormError(
                affected, "Новая ошибка формы", "ДиаметрНожки");

            string finalError = parameters.GetErrorMessages();
            var expectedError = "не находится в диапазоне";

            Assert.Multiple(() =>
            {
                Assert.That(finalError, Does.Contain("Новая ошибка формы"));
                Assert.That(finalError, Does.Not.Contain(expectedError));
                Assert.That(finalError.Split('\n').Length, Is.EqualTo(1));
            });
        }

        [Test]
        [Description(
            "GetErrorMessages должен форматировать сообщение "
            + "без имени поля")]
        public void GetErrorMessages_ShouldFormatMessageWithoutFieldName()
        {
            var parameters = new Parameters();
            // Вне диапазона [3, 6]
            parameters.SetValue(ParameterType.LegCountC, 10);
            string errorMessage = parameters.GetErrorMessages();

            Assert.Multiple(() =>
            {
                var expected =
                    "Значение 10 не находится в диапазоне [3, 6]";
                Assert.That(errorMessage, Is.EqualTo(expected));
                Assert.That(errorMessage, Does.Not.Contain(":"));
            });
        }

        #endregion
    }
}
