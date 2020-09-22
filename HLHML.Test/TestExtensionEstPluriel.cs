﻿using Shouldly;
using Xunit;
using static HLHML.Dictionnaire.TermeBuilder;

namespace HLHML.Test
{
    public class TestExtensionEstPluriel
    {
        [Fact]
        public void RègleGénérale()
        {
            Terme("boîte", TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme("boîtes", TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Fact]
        public void LesNomsEn_ail()
        {
            Terme("détail", TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme("détails", TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Theory]
        [InlineData("bail", "baux")]
        [InlineData("émail", "émaux")]
        [InlineData("corail", "coraux")]
        [InlineData("soupirail", "soupiraux")]
        [InlineData("travail", "travaux")]
        [InlineData("vantail", "vantaux")]
        [InlineData("vitrail", "vitraux")]
        public void LesNomsEn_ail_Exceptions(string singulier, string pluriel)
        {
            Terme(singulier, TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme(pluriel, TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Fact]
        public void LesNomsEn_ou()
        {
            Terme("trou", TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme("trous", TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Theory]
        [InlineData("bijou", "bijoux")]
        [InlineData("caillou", "cailloux")]
        [InlineData("chou", "choux")]
        [InlineData("genou", "genoux")]
        [InlineData("hibou", "hiboux")]
        [InlineData("joujou", "joujoux")]
        [InlineData("pou", "poux")]
        public void LesNomsEn_ou_Exceptions(string singulier, string pluriel)
        {
            Terme(singulier, TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme(pluriel, TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Fact]
        public void LesNomsEn_al()
        {
            Terme("journal", TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme("journaux", TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Theory]
        [InlineData("aval", "avals")]
        [InlineData("bal", "bals")]
        [InlineData("cal", "cals")]
        [InlineData("carnaval", "carnavals")]
        [InlineData("cérémonial", "cérémonials")]
        [InlineData("chacal", "chacals")]
        [InlineData("choral", "chorals")]
        [InlineData("festival", "festivals")]
        [InlineData("régal", "régals")]
        [InlineData("récital", "récitals")]
        public void LesNomsEn_al_Exceptions(string singulier, string pluriel)
        {
            Terme(singulier, TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme(pluriel, TokenType.Nom).EstPluriel().ShouldBeTrue();
        }

        [Theory]
        [InlineData("au", "aux")]
        [InlineData("eau", "eaux")]
        [InlineData("eu", "eux")]
        [InlineData("tuyau", "tuyaux")]
        [InlineData("poteau", "poteaux")]
        [InlineData("cheveu", "cheveux")]
        [InlineData("landau", "landaus")]
        [InlineData("sarrau", "sarraus")]
        [InlineData("bleu", "bleus")]
        [InlineData("pneu", "pneus")]
        [InlineData("lieu", "lieus")]
        public void LesNoms_au_eau_eu(string singulier, string pluriel)
        {
            Terme(singulier, TokenType.Nom).EstPluriel().ShouldBeFalse();
            Terme(pluriel, TokenType.Nom).EstPluriel().ShouldBeTrue();
        }
    }
}
