using PdfSharp.Fonts;
using System.Reflection;

namespace CashFlow.Application.UseCases.Expenses.Reports.PDF.Fonts;

public class ExpensesReportFontsResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        var stream = ReadFontFile(faceName);

        stream ??= ReadFontFile(FontHelper.DEFAULT_FONT);//caso sejá nulo, será atribuída a fonte padrão definida no FontHelper.cs

        var lenght = (int)stream!.Length; //vai retornar quantos bytes tem essa stream em int em vez de long. '!' indica que temos a certeza que o resultado não será nulo

        var data = new byte[lenght]; //vai criar um array de bytes já passando o tamanho.

        stream.Read(buffer: data, offset: 0, count: lenght);


        return data;
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
      

        return new FontResolverInfo(familyName);
    }

    private Stream? ReadFontFile(string faceName) //caso passe o caminho errado ou uma fonte que não exista na pasta, o resultado poderá ser nulo
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceStream($"CashFlow.Application.UseCases.Expenses.Reports.PDF.Fonts.{faceName}.ttf");
    }
}
