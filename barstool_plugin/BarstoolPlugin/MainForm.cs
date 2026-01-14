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
        /// Построитель 3D-модели диска.
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
        /// Инициализирует поля формы и связывает элементы управления с
        /// параметрами.
        /// </summary>
        private void InitializeFields()
        {
            InitializeTextBox(legDiameterD1TextBox,
                ParameterType.LegDiameterD1, "Диаметр ножки");
            InitializeTextBox(footrestDiameterD2TextBox,
                ParameterType.FootrestDiameterD2, "Диаметр подножки");
            InitializeTextBox(seatDiameterDTextBox,
                ParameterType.SeatDiameterD, "Диаметр сидения");
            InitializeTextBox(footrestHeightH1TextBox,
                ParameterType.FootrestHeightH1, "Высота подножки");
            InitializeTextBox(stoolHeightHTextBox,
                ParameterType.StoolHeightH, "Высота стула");
            InitializeTextBox(seatDepthSTextBox,
                ParameterType.SeatDepthS, "Вылет сидения");

            UpdateErrorMessageDisplay();
        }

        /// <summary>
        /// Инициализирует TextBox для ввода параметра модели.
        /// </summary>
        /// <param name="textBox">Элемент управления TextBox для
        /// инициализации.</param>
        /// <param name="paramType">Тип параметра, связанного с
        /// TextBox.</param>
        /// <param name="label">Отображаемое имя параметра для сообщений
        /// об ошибках.</param>
        private void InitializeTextBox(TextBox textBox,
            ParameterType paramType, string label)
        {
            _textBoxMappings[textBox] = paramType;

            textBox.TextChanged += DataChanged;
            int defaultValue = _parameters.GetValue(paramType);
            textBox.Text = defaultValue.ToString();
            textBox.Tag = label;
        }

        /// <summary>
        /// Обрабатывает изменение значения параметра.
        /// </summary>
        private void DataChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text.Trim();
                string fieldName = textBox.Tag.ToString();

                _parameters.RemoveErrorsForParameter(
                    _textBoxMappings[textBox]);

                if (string.IsNullOrEmpty(text))
                {
                    _parameters.AddFormError(
                        new List<ParameterType> {
                            _textBoxMappings[textBox] },
                        "Поле должно быть заполнено",
                        fieldName
                    );
                }
                else if (!int.TryParse(text, out int value))
                {
                    _parameters.AddFormError(
                        new List<ParameterType> {
                            _textBoxMappings[textBox] },
                        "Значение должно являться целым числом",
                        fieldName
                    );
                }
                else
                {
                    _parameters.SetValue(_textBoxMappings[textBox],
                        value, fieldName);
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
            var parametersWithErrors = _parameters
                .GetAllParametersWithErrors();

            foreach (var mapping in _textBoxMappings)
            {
                TextBox textBox = mapping.Key;
                ParameterType paramType = mapping.Value;

                bool hasParameterErrors = parametersWithErrors
                    .Contains(paramType);

                if (!hasParameterErrors)
                {
                    textBox.BackColor = SystemColors.Window;
                }
                else
                {
                    textBox.BackColor = SystemColors.ActiveCaption;
                }
            }
        }

        /// <summary>
        /// Обновляет отображение сообщений об ошибках в richTextBox.
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

                    MessageBox.Show(
                        "Пожалуйста, исправьте ошибки перед построением" +
                        " модели:\n\n" + errorMessages,
                        "Ошибка валидации", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _builder.Build(_parameters);

                MessageBox.Show(
                    "Модель успешно построена в КОМПАС-3D!",
                    "Успех", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при построении:\n\n{ex.Message}",
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