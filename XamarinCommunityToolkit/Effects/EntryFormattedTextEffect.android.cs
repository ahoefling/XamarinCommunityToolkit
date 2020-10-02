﻿using System.ComponentModel;
using Android.Text;
using Android.Text.Style;
using Android.Widget;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(nameof(Xamarin.CommunityToolkit))]
[assembly: ExportEffect(typeof(EntryFormattedTextEffect), nameof(EntryFormattedTextEffect))]

namespace Xamarin.CommunityToolkit.Effects
{
	/// <summary>
	/// The FormattedText <see cref="PlatformEffect"/> specific
	/// to <see cref="Entry"/> controls.
	/// </summary>
	public class EntryFormattedTextEffect : PlatformEffect
	{
		/// <summary>
		/// Gets the native <see cref="FormsEditText"/> control.
		/// </summary>
		public new FormsEditText Control => (FormsEditText)base.Control;

		/// <summary>
		/// Gets the Xamarin.Forms <see cref="Entry"/> element.
		/// </summary>
		public new Entry Element => (Entry)base.Element;

		/// <inheritdoc />
		protected override void OnAttached()
		{
			UpdateFormattedText();
		}

		/// <inheritdoc />
		protected override void OnDetached()
		{
		}

		/// <inheritdoc />
		protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(args);

			if (args.PropertyName == FormattedTextEffect.FormattedTextProperty.PropertyName)
			{
				UpdateFormattedText();
			}
		}

		/// <summary>
		/// Updates the native <see cref="FormsEditText"/> with the
		/// Text or FormattedText. If the FormattedText is null the
		/// Control defaults to using Text property. Otherwise it uses
		/// the custom Spans in FormattedText.
		/// </summary>
		void UpdateFormattedText()
		{
			var selectionStart = Control.SelectionStart;
			var selectionEnd = Control.SelectionEnd;
			var formattedText = FormattedTextEffect.GetFormattedText(Element);

			if (formattedText == null)
			{
				Control.Text = Element.Text;
			}
			else
			{
				var text = new SpannableStringBuilder();
				var position = 0;
				for (var index = 0; index < formattedText.Spans.Count; index++)
				{
					var currentSpan = formattedText.Spans[index];
					text.Append(currentSpan.Text);

					var nativeSpan = CreateNativeSpan(currentSpan);
					text.SetSpan(nativeSpan, position, position + currentSpan.Text.Length, SpanTypes.ExclusiveExclusive);

					position += currentSpan.Text.Length;
				}

				Control.SetText(text, TextView.BufferType.Spannable);
			}

			Control.SetSelection(selectionStart, selectionEnd);
		}

		/// <summary>
		/// Create a native Android Span that is used
		/// with the <see cref="Spannable"/> object.
		/// </summary>
		/// <param name="span">
		/// The Xamarin.Forms <see cref="Span"/> to convert to a native span.
		/// </param>
		/// <returns>
		/// The native Android Span.
		/// </returns>
		/// <remarks>
		/// The native Android <see cref="Spannable"/> accepts <see cref="Java.Lang.Object"/>
		/// which allows several different types. Please refer to the Android Documentation
		/// on usage. https://developer.android.com/reference/android/text/Spannable.
		/// </remarks>
		protected virtual Java.Lang.Object CreateNativeSpan(Span span)
		{
			return new ForegroundColorSpan(span.TextColor.ToAndroid());
		}
	}
}
