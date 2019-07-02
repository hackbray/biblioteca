using BibliotecaDominio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaDominio
{
    public class Bibliotecario
    {
        public const string EL_LIBRO_NO_SE_ENCUENTRA_DISPONIBLE = "El libro no se encuentra disponible";
        public const string LOS_LIBROS_PALINDROMOS_SOLO_EN_LA_BIBLIOTECA = "Los libros palíndromos solo se pueden utilizar en la biblioteca";
        public const int DIAS_MAXIMO_DE_ENTREGA = 15;
        public const int NUMERO_MAXIMO = 30;
        private IRepositorioLibro libroRepositorio;
        private IRepositorioPrestamo prestamoRepositorio;

        public Bibliotecario() { }

        public Bibliotecario(IRepositorioLibro libroRepositorio, IRepositorioPrestamo prestamoRepositorio)
        {
            this.libroRepositorio = libroRepositorio;
            this.prestamoRepositorio = prestamoRepositorio;
        }

        public void Prestar(string isbn, string nombreUsuario)
        {
            var libroExistenciaVerificada = libroRepositorio.ObtenerPorIsbn(isbn);
            if (libroExistenciaVerificada != null)
            {
                if (EsPrestado(isbn))
                {
                    if (!EsPalindroma(isbn))
                    {
                        prestamoRepositorio.Agregar(new Prestamo(DateTime.Now, libroExistenciaVerificada, CalcularFechaEntregaMaxima(isbn), nombreUsuario));
                    }
                    else
                    {
                        Console.WriteLine(LOS_LIBROS_PALINDROMOS_SOLO_EN_LA_BIBLIOTECA);
                        throw new Exception(LOS_LIBROS_PALINDROMOS_SOLO_EN_LA_BIBLIOTECA);
                    }
                }
                else
                {
                    Console.WriteLine(EL_LIBRO_NO_SE_ENCUENTRA_DISPONIBLE);
                    throw new Exception(EL_LIBRO_NO_SE_ENCUENTRA_DISPONIBLE);
                }
            }
            else
            {
                Console.WriteLine(EL_LIBRO_NO_SE_ENCUENTRA_DISPONIBLE);
                throw new Exception(EL_LIBRO_NO_SE_ENCUENTRA_DISPONIBLE);
            }
        }

        public static DateTime CalcularFechaEntregaMaxima(String isbn)
        {
            DateTime fechaMaxima = new DateTime();
            if (EsMayorQue(NUMERO_MAXIMO, isbn))
            {
                for (int k = 1; k < DIAS_MAXIMO_DE_ENTREGA; k++)
                {
                    while (fechaMaxima.DayOfWeek == DayOfWeek.Sunday) fechaMaxima = fechaMaxima.AddDays(1);
                    fechaMaxima = fechaMaxima.AddDays(1);
                }
            }
            return fechaMaxima;
        }

        public static bool EsMayorQue(int maximo, string isbn)
        {
            int sumaDeNumeros = 0;
            bool esMayor = false;

            foreach (char caracter in isbn)
            {
                if (char.IsDigit(caracter))
                    sumaDeNumeros += Convert.ToInt32(caracter.ToString());
            }
            esMayor = sumaDeNumeros > maximo ? true : false;
            return esMayor;
        }

        public bool EsPrestado(string isbn)
        {
            var libroPrestado = prestamoRepositorio.ObtenerLibroPrestadoPorIsbn(isbn);
            if (libroPrestado != null)
                return true;
            else
                return false;
        }

        public static bool EsPalindroma(String cadena)
        {
            if (cadena.Length < 2) return true;
            if (cadena[0] == cadena[cadena.Length - 1]) return EsPalindroma(cadena.Substring(1, cadena.Length - 2));
            return false;
        }
    }
}
