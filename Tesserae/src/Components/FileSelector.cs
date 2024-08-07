﻿using static H5.Core.dom;
using static Tesserae.UI;

namespace Tesserae
{
    [H5.Name("tss.FileSelector")]
    public sealed class FileSelector : IComponent, ICanValidate<FileSelector>
    {
        public delegate void              FileSelectedHandler(FileSelector sender, File file);
        private event FileSelectedHandler FileSelected;

        private readonly HTMLInputElement _fileInput;
        private readonly IComponent       _stack;
        private readonly TextBox          _textBox;
        private readonly HTMLElement      _container;
        private          File             _selectedFile;

        public File SelectedFile
        {
            get => _selectedFile;
            private set
            {
                _selectedFile = value;
                FileSelected?.Invoke(this, value);
            }
        }

        public string Placeholder
        {
            get => _textBox.Placeholder;
            set => _textBox.Placeholder = value;
        }

        public string Error
        {
            get => _textBox.Error;
            set => _textBox.Error = value;
        }

        public bool IsInvalid
        {
            get => _textBox.IsInvalid;
            set => _textBox.IsInvalid = value;
        }

        public bool IsRequired
        {
            get => _textBox.IsRequired;
            set => _textBox.IsRequired = value;
        }


        /// <summary>
        /// Gets or sets the type of files accepted by this selector. See https://www.w3schools.com/tags/att_input_accept.asp for more information.
        /// Valid values are a list of extensions, like ".txt|.doc|.docx", of media type, such as  "audio/*|video/*|image/*", or a combination of both
        /// </summary>
        public string Accepts
        {
            get => _fileInput.accept;
            set => _fileInput.accept = value;
        }

        public FileSelector()
        {
            _fileInput = FileInput(_("tss-file-input"));
            _textBox   = TextBox().ReadOnly().Grow(1).AlignCenter();

            _stack = Stack().Horizontal().WS()
               .Children(_textBox,
                    Button().SetTitle("Click to select file...").NoWrap().SetIcon(UIcons.Folder).OnClick((s, e) => _fileInput.click()).NoBorder().NoBackground(),
                    Raw(_fileInput));

            _fileInput.onchange = _ => updateFile();

            _container = Div(_("tss-fileselector"), _stack.Render());

            void updateFile()
            {
                if (_fileInput.files.length > 0)
                {
                    SelectedFile  = _fileInput.files[0];
                    _textBox.Text = GetFileName(_fileInput.value);
                }
            }
            ;
        }

        public FileSelector OnFileSelected(FileSelectedHandler handler)
        {
            FileSelected += handler;
            return this;
        }

        public FileSelector SetPlaceholder(string placeholder)
        {
            Placeholder = placeholder;
            return this;
        }

        /// <summary>
        /// Sets the type of files accepted by this selector. See https://www.w3schools.com/tags/att_input_accept.asp for more information.
        /// Valid values are a list of extensions, like ".txt|.doc|.docx", of media type, such as  "audio/*|video/*|image/*", or a combination of both
        /// </summary>
        /// <param name="accepts"></param>
        /// <returns></returns>
        public FileSelector SetAccepts(string accepts)
        {
            Accepts = accepts;
            return this;
        }

        public FileSelector Required()
        {
            IsRequired = true;
            return this;
        }

        public void Reset()
        {
            _fileInput.value = null;
        }

        public void Attach(ComponentEventHandler<FileSelector> handler)
        {
            FileSelected += (s, _) => handler(s);
        }

        private string GetFileName(string value)
        {
            var lastSep = value.LastIndexOfAny(new[] { '/', '\\' });
            return value.Substring(lastSep + 1);
        }

        public HTMLElement Render()
        {
            return _container;
        }
    }
}