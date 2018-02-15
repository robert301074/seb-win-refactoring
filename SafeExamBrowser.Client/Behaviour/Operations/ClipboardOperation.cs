﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.Contracts.Behaviour.Operations;
using SafeExamBrowser.Contracts.I18n;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Contracts.UserInterface;
using SafeExamBrowser.Contracts.WindowsApi;

namespace SafeExamBrowser.Client.Behaviour.Operations
{
	internal class ClipboardOperation : IOperation
	{
		private ILogger logger;
		private INativeMethods nativeMethods;

		public bool Abort { get; private set; }
		public IProgressIndicator ProgressIndicator { private get; set; }

		public ClipboardOperation(ILogger logger, INativeMethods nativeMethods)
		{
			this.logger = logger;
			this.nativeMethods = nativeMethods;
		}

		public void Perform()
		{
			EmptyClipboard();
		}

		public void Repeat()
		{
			// Nothing to do here...
		}

		public void Revert()
		{
			EmptyClipboard();
		}

		private void EmptyClipboard()
		{
			logger.Info("Emptying clipboard...");
			ProgressIndicator?.UpdateText(TextKey.ProgressIndicator_EmptyClipboard);

			nativeMethods.EmptyClipboard();
		}
	}
}