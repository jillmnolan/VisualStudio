﻿using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using GitHub.Commands;
using GitHub.Services;
using GitHub.InlineReviews.Commands;

namespace GitHub.InlineReviews
{
    /// <summary>
    /// Export a <see cref="IWpfTextViewMarginProvider"/>, which returns an instance of the margin for the editor to use.
    /// </summary>
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(CommentsMargin.MarginName)]
    [Order(After = PredefinedMarginNames.ZoomControl)]
    [MarginContainer(PredefinedMarginNames.BottomControl)]             // Set the container to the bottom of the editor window
    [ContentType("text")]                                       // Show this margin for all text-based types
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class CommentsMarginFactory : IWpfTextViewMarginProvider
    {
        readonly IEnableInlineCommentsCommand enableInlineCommentsCommand;
        readonly INextInlineCommentCommand nextInlineCommentCommand;
        readonly IPullRequestSessionManager sessionManager;

        [ImportingConstructor]
        public CommentsMarginFactory(IEnableInlineCommentsCommand enableInlineCommentsCommand, INextInlineCommentCommand nextInlineCommentCommand, IPullRequestSessionManager sessionManager)
        {
            this.enableInlineCommentsCommand = enableInlineCommentsCommand;
            this.nextInlineCommentCommand = nextInlineCommentCommand;
            this.sessionManager = sessionManager;
        }

        /// <summary>
        /// Creates an <see cref="IWpfTextViewMargin"/> for the given <see cref="IWpfTextViewHost"/>.
        /// </summary>
        /// <param name="wpfTextViewHost">The <see cref="IWpfTextViewHost"/> for which to create the <see cref="IWpfTextViewMargin"/>.</param>
        /// <param name="marginContainer">The margin that will contain the newly-created margin.</param>
        /// <returns>The <see cref="IWpfTextViewMargin"/>.
        /// The value may be null if this <see cref="IWpfTextViewMarginProvider"/> does not participate for this context.
        /// </returns>
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            // Never show on diff views
            if (IsDiffView(wpfTextViewHost.TextView))
            {
                return null;
            }

            return new CommentsMargin(wpfTextViewHost.TextView, enableInlineCommentsCommand, nextInlineCommentCommand, sessionManager);
        }

        bool IsDiffView(ITextView textView) => textView.Roles.Contains("DIFF");
    }
}
