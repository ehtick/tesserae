﻿using System;
using static Tesserae.UI;

namespace Tesserae
{

    [H5.Name("tss.ProgressModal")]
    public class ProgressModal
    {
        private readonly Modal             _modalHost;
        private readonly Raw               _titleHost;
        private readonly Raw               _messageHost;
        private readonly Raw               _progressHost;
        private readonly Raw               _footerHost;
        private readonly ProgressIndicator _progressIndicator;
        private readonly Spinner           _spinner;
        private          bool              _isSpinner = true;

        public ProgressModal()
        {
            _titleHost    = Raw().WS();
            _messageHost  = Raw().WS();
            _footerHost   = Raw().WS();
            _progressHost = Raw();
            _spinner      = Spinner().Large().Margin(8.px());
            _progressHost.Content(_spinner);
            _progressIndicator = ProgressIndicator();
            _isSpinner         = true;

            _modalHost = Modal().Blocking().NoLightDismiss().HideCloseButton().CenterContent()
               .Content(Stack()
                   .AlignCenter()
                   .WS()
                   .Children(_titleHost, _progressHost, _messageHost, _footerHost));

        }

        public ProgressModal Show()
        {
            _modalHost.Show();
            return this;
        }


        public IComponent ShowEmbedded()
        {
            return _modalHost.ShowEmbedded();
        }

        public ProgressModal Hide()
        {
            _modalHost.Hide();
            return this;
        }

        public ProgressModal Message(string message)
        {
            _messageHost.Content(TextBlock(message));
            return this;
        }

        public ProgressModal Message(IComponent message)
        {
            _messageHost.Content(message);
            return this;
        }

        public ProgressModal Title(string title)
        {
            _titleHost.Content(TextBlock(title).SemiBold().Primary().PaddingTop(16.px()).PaddingBottom(8.px()));
            return this;
        }

        public ProgressModal Title(IComponent title)
        {
            _titleHost.Content(title);
            return this;
        }

        public ProgressModal Progress(float percent)
        {
            if (_isSpinner)
            {
                _progressHost.Content(_progressIndicator);
                _isSpinner = false;
            }
            _progressIndicator.Progress(percent);
            return this;
        }

        public ProgressModal Progress(int position, int total) => Progress(100f * position / total);

        public ProgressModal ProgressIndeterminated()
        {
            if (_isSpinner)
            {
                _progressHost.Content(_progressIndicator);
                _isSpinner = false;
            }
            _progressIndicator.Indeterminated();
            return this;
        }

        public ProgressModal ProgressSpin()
        {
            if (!_isSpinner)
            {
                _progressHost.Content(_spinner);
                _isSpinner = true;
            }
            return this;
        }

        public ProgressModal WithCancel(Action<Button> onCancel, Action<Button> btnCancel = null)
        {
            var button = Button().SetText("Cancel").SetIcon(UIcons.Cross).Danger();
            btnCancel?.Invoke(button);
            button.OnClick((b, __) => onCancel(b));
            _footerHost.PaddingTop(16.px()).Content(button.AlignCenter());
            return this;
        }
    }
}