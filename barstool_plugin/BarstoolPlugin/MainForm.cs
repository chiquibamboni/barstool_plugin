using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BarstoolPlugin.Services;
using BarstoolPluginCore.Model;

namespace BarstoolPlugin
{
    /// <summary>
    /// Главная форма плагина для построения барного стула.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Объект 3D-модели.
        /// </summary>
        private Builder _builder;

        /// <summary>
        /// Параметры модели.
        /// </summary>
        private Parameters _parameters;

        /// <summary>
        /// Словарь для связи TextBox с ParameterType.
        /// </summary>
        private Dictionary<TextBox, ParameterType> _textBoxMappings;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public MainForm()
        {
            _builder = new Builder();
            _parameters = new Parameters();
            _textBoxMappings = new Dictionary<TextBox, ParameterType>();

            InitializeComponent();
            InitializeFields();
        }

        /// <summary>
        /// Инициализирует поля формы и связывает элементы управления
        /// с параметрами.
        /// </summary>
        private void InitializeFields()
        {
            InitializeTextBox(legDiameterD1TextBox,
                legDiameterD1LimitLabel, ParameterType.LegDiameterD1,
                "Диаметр ножки");
            InitializeTextBox(footrestDiameterD2TextBox,
                footrestDiameterD2LimitLabel,
                ParameterType.FootrestDiameterD2, "Диаметр подножки");
            InitializeTextBox(seatDiameterDTextBox,
                seatDiameterDLimitLabel, ParameterType.SeatDiameterD,
                "Диаметр сидения");
            InitializeTextBox(footrestHeightH1TextBox,
                footrestHeightH1LimitLabel,
                ParameterType.FootrestHeightH1, "Высота подножки");
            InitializeTextBox(stoolHeightHTextBox,
                stoolHeightHLimitLabel, ParameterType.StoolHeightH,
                "Высота стула");
            InitializeTextBox(seatDepthSTextBox,
                seatDepthSLimitLabel, ParameterType.SeatDepthS,
                "Вылет сидения");
            InitializeTextBox(legCountCTextBox,
                legCountCLimitLabel, ParameterType.LegCountC,
                "Количество ножек");

            UpdateErrorMessageDisplay();
        }

        /// <summary>
        /// Инициализирует TextBox и Lable для ввода параметра модели.
        /// </summary>
        /// <param name="textBox">Элемент управления TextBox для ввода
        /// значения параметра</param>
        /// <param name="limitLabel">Элемент управления Label для отображения
        /// допустимых пределов параметра</param>
        /// <param name="paramType">Тип параметра (enum ParameterType)</param>
        /// <param name="paramName">Отображаемое имя параметра
        /// для пользователя</param>
        private void InitializeTextBox(TextBox textBox, Label limitLabel,
            ParameterType paramType, string paramName)
        {
            _textBoxMappings[textBox] = paramType;
            textBox.TextChanged += DataChanged;

            int defaultValue = _parameters.GetValue(paramType);
            int minValue = _parameters.GetMin(paramType);
            int maxValue = _parameters.GetMax(paramType);

            textBox.Tag = paramName;
            textBox.Text = defaultValue.ToString();

            
            limitLabel.Text = $"от {minValue} до {maxValue} " +
                //TODO: refactor
                limitLabel.Name == "legCountCLimitLabel" ? "шт" : "мм";
        }

        /// <summary>
        /// Обрабатывает изменение значения параметра.
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события изменения текста</param>
        private void DataChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text.Trim();
                string fieldName = textBox.Tag.ToString();
                var paramType = _textBoxMappings[textBox];

                _parameters.RemoveErrorsForParameter(paramType);

                if (string.IsNullOrEmpty(text))
                {
                    _parameters.AddFormError(
                        new List<ParameterType> { paramType },
                        "Поле должно быть заполнено", fieldName);
                }
                else if (!int.TryParse(text, out int value))
                {
                    _parameters.AddFormError(
                        new List<ParameterType> { paramType },
                        "Значение должно являться целым числом", fieldName);
                }
                else
                {
                    _parameters.SetValue(paramType, value, fieldName);
                }
                UpdateTextBoxesColors();
                UpdateErrorMessageDisplay();
            }
        }

        /// <summary>
        /// Обновляет цвет текстбоксов на основе текущих ошибок.
        /// </summary>
        private void UpdateTextBoxesColors()
        {
            var parametersWithErrors =
                _parameters.GetAllParametersWithErrors();

            foreach (var mapping in _textBoxMappings)
            {
                TextBox textBox = mapping.Key;
                ParameterType paramType = mapping.Value;
                bool hasParameterErrors =
                    parametersWithErrors.Contains(paramType);

                textBox.BackColor = hasParameterErrors
                    ? SystemColors.ActiveCaption
                    : SystemColors.Window;
            }
        }

        /// <summary>
        /// Обновляет отображение сообщений об ошибках.
        /// </summary>
        private void UpdateErrorMessageDisplay()
        {
            richTextBox.Clear();

            if (_parameters.HasErrors)
            {
                richTextBox.BackColor = SystemColors.ActiveCaption;
                richTextBox.ForeColor = Color.Black;
                richTextBox.Text = _parameters.GetErrorMessages();
            }
            else
            {
                richTextBox.BackColor = SystemColors.Control;
                richTextBox.ForeColor = SystemColors.WindowText;
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки построения модели.
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события нажатия кнопки</param>
        private void BuildButton_Click(object sender, EventArgs e)
        {
            BuildModel();
        }

        /// <summary>
        /// Выполняет построение модели барного стула.
        /// </summary>
        private void BuildModel()
        {
            try
            {
                BuildButton.Enabled = false;
                BuildButton.Text = "Построение...";
                Application.DoEvents();

                if (_parameters.HasErrors)
                {
                    string errorMessages = _parameters.GetErrorMessages();
                    var message = "Пожалуйста, исправьте ошибки перед "
                        + "построением модели:\n\n" + errorMessages;
                    MessageBox.Show(message, "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _builder.Build(_parameters);
                MessageBox.Show("Модель успешно построена в КОМПАС-3D!",
                    "Успех", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении:\n\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BuildButton.Enabled = true;
                BuildButton.Text = "Построить";
            }
        }
    }
}