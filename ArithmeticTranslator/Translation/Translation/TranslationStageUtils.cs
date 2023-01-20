using System;
using System.Collections.Generic;

namespace Translation.Translation
{
    /// <summary>
    /// Утилита, работающая с этапами трансляции
    /// </summary>
    public static class TranslationStageUtils
    {
        /// <summary>
        /// Список связей строковых представлений с этапами трансляции
        /// </summary>
        private static readonly Dictionary<string, TranslationStage> stagesInfo = new Dictionary<string, TranslationStage>()
        {
            { "lex", TranslationStage.LEXICAL_ANALYSIS },
            { "syn", TranslationStage.SYNTAX_ANALYSIS },
            { "sem", TranslationStage.SEMANTIC_ANALYSIS },
            { "gen1", TranslationStage.PORTABLE_CODE_GENERATION },
            { "gen2", TranslationStage.POSTFIX_GENERATION },
            { "gen3", TranslationStage.BINARY_CODE_GENERATION }
        };

        /// <summary>
        /// Получает этап трансляции по строковому представлению
        /// </summary>
        /// <param name="stageStr">Строковое представление этапа</param>
        /// <returns>Этап трансляции</returns>
        public static TranslationStage GetTranslationStage(string stageStr)
        {
            stageStr = stageStr.ToLower();
            if (stagesInfo.ContainsKey(stageStr))
            {
                return stagesInfo[stageStr];
            }
            throw new Exception($"Этапа трансляции с именем \"{stageStr}\" не существует!");
        }
    }
}