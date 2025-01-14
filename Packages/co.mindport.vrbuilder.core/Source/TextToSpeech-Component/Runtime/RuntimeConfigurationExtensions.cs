﻿using System;
using System.Linq;
using Source.TextToSpeech_Component.Runtime;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Utils;

namespace VRBuilder.TextToSpeech
{
    /// <summary>
    /// TextToSpeech extensions methods for <see cref="BaseRuntimeConfiguration"/>.
    /// </summary>
    public static class RuntimeConfigurationExtensions
    {
        /// <summary>
        /// Text to speech configuration.
        /// </summary>
        private static ITextToSpeechConfiguration TextToSpeechConfiguration;

        /// <summary>
        /// Return loaded <see cref="ITextToSpeechConfiguration"/>.
        /// </summary>
        public static ITextToSpeechConfiguration GetTextToSpeechConfiguration(this BaseRuntimeConfiguration runtimeConfiguration)
        {
            return GetTextToSpeechSettings(runtimeConfiguration).Configuration ?? MicrosoftTextToSpeechConfiguration.Instance;
        }

        public static TextToSpeechSettings GetTextToSpeechSettings(this BaseRuntimeConfiguration runtimeConfiguration)
        {
            return TextToSpeechSettings.Instance;
        }

        /// <summary>
        /// Loads a new <see cref="MicrosoftTextToSpeechConfiguration"/>
        /// </summary>
        public static void SetTextToSpeechConfiguration(this BaseRuntimeConfiguration runtimeConfiguration,
            ITextToSpeechConfiguration textToSpeechConfiguration)
        {
            TextToSpeechConfiguration = textToSpeechConfiguration;
        }
    }
}