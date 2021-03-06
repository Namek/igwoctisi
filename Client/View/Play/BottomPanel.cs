﻿namespace Client.View.Play
{
	using System;
	using Client.Common;
	using Client.View.Controls;
	using Nuclex.UserInterface;
	using Nuclex.UserInterface.Controls;
	using Nuclex.UserInterface.Controls.Desktop;

	public class BottomPanel : TabbedPaneControl
	{
		public event EventHandler ChatMessageSent;

		public bool IsFlashing { get; set; }

		public BottomPanel() 
			: base(new UniRectangle(new UniScalar(0.0f, 32), new UniScalar(1.0f, 0), new UniScalar(0.4f, 0.0f), new UniScalar(0.3f, 40)))
		{
			DefaultPosition = new UniVector(new UniScalar(0, 32), new UniScalar(0.7f, -40));
			TogglePosition = new UniVector(new UniScalar(0.0f, 32), new UniScalar(1.0f, -40));
			IsFlashing = false;
			Toggled += Toggled_Handler;

			var panel = new LabelControl
			{
				Bounds = new UniRectangle(new UniScalar(), new UniScalar(), new UniScalar(1.0f, 0), new UniScalar(1.0f, 0))
			};

			_messageList = new WrappableListControl
			{
				SelectionMode = ListSelectionMode.None,
				Bounds = new UniRectangle(new UniScalar(0.03f, 0), new UniScalar(0.05f, 0), new UniScalar(0.94f, 0), new UniScalar(0.775f, 0))
			};

			_chatMessage = new CommandInputControl
			{
				Bounds = new UniRectangle(new UniScalar(0.03f, 0), new UniScalar(0.825f, 0), new UniScalar(0.87f, 0), new UniScalar(0.125f, 0))
			};
			_chatMessage.OnCommandHandler += new EventHandler(ChatMessage_Execute);

			var btnClearMessage = new ButtonControl
			{
				Text = "C",
				Bounds = new UniRectangle(new UniScalar(0.9f, 0), new UniScalar(0.825f, 0), new UniScalar(0.075f, 0), new UniScalar(0.125f, 0))
			};
			btnClearMessage.Pressed += ClearMessageList;
			panel.Children.AddRange(new Control[] { _chatMessage, _messageList, btnClearMessage });

			var chatIcon = new string[] { "chatIconPulsing", "chatIconActive", "chatIconHover", "chatIconPushed" };
			var chatIconPulse = new string[] { "chatIconPulsing", "chatIconPulsing", "chatIconPulsing", "chatIconPulsing" };
			AddTab(chatIcon, chatIconPulse, panel);
		}

		private void Toggled_Handler(object sender, EventArgs e)
		{
			if (!IsToggled && IsFlashing)
			{
				IsFlashing = false;
			}
		}

		#region Update functions

		public void AddMessage(string message)
		{
			_messageList.AddItem(message);
		}

		public void ToggleChatButtonFlash()
		{
			var chatButton = _tabHeaderPanel.Children[1] as TabImageHeaderControl;
			chatButton.Selected = IsToggled ? !chatButton.Selected : true;
		}

		#endregion

		#region Event handlers

		private void ClearMessageList(object sender, EventArgs e)
		{
			_messageList.Clear();
		}

		private void ChatMessage_Execute(object sender, EventArgs e)
		{
			if (ChatMessageSent != null)
			{
				ChatMessageSent(_chatMessage, e);
			}
		}

		#endregion

		private WrappableListControl _messageList;
		private CommandInputControl _chatMessage;
	}
}
