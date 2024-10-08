﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public partial class DatosInsert
    {
        public enum typo
        {
            Text, 
            Num, 
            Bool,
            Custom
        }

        private string campo;
        private string tipo;

        //Comun
        private int longitud;
        private bool aleatorio;

        //Text
        private int maximo;
        private int mininmo;
        private bool fijo;
        private bool minusculas;
        private bool mayusculas;
        private bool numeros;
        private bool caracteresespeciales;
        private bool primermayuscula;
        private bool repetir;
        private string texto;

        //Num
        private bool numdecimal;
        private int decimalesmax;
        private bool nocero;
        private bool autoincremento;
        private bool rango;

        //Bool
        private bool trueFalse;

        public string Campo { get => campo; set => campo = value; }
        public string Tipo { get => tipo; set => tipo = value; }
        public bool Aleatorio { get => aleatorio; set => aleatorio = value; }
        public int Longitud { get => longitud; set => longitud = value; }
        public int Maximo { get => maximo; set => maximo = value; }
        public int Minimo { get => mininmo; set => mininmo = value; }
        public bool Fijo { get => fijo; set => fijo = value; }
        public bool Minusculas { get => minusculas; set => minusculas = value; }
        public bool Mayusculas { get => mayusculas; set => mayusculas = value; }
        public bool Numeros { get => numeros; set => numeros = value; }
        public bool Caracteresespeciales { get => caracteresespeciales; set => caracteresespeciales = value; }
        public bool Primermayuscula { get => primermayuscula; set => primermayuscula = value; }
        public bool Repetir { get => repetir; set => repetir = value; }
        public string Texto { get => texto; set => texto = value; }
        public bool TrueFalse { get => trueFalse; set => trueFalse = value; }
        public bool NumDecimal { get => numdecimal; set => numdecimal = value; }
        public int Decimalesmax { get => decimalesmax; set => decimalesmax = value; }
        public bool NoCero { get => nocero; set => nocero = value; }
        public bool Autoincremento { get => autoincremento; set => autoincremento = value; }
        public bool Rango { get => rango; set => rango = value; }

        public DatosInsert(string _campo, string _tipo)
        {
            Tipo = _tipo;
            Campo = _campo;
        }

        public DatosInsert(string _campo)
        {
            Campo = _campo;
        }

        public DatosInsert()
        {
            Aleatorio = true;
            Fijo = true;
        }
    }
}
