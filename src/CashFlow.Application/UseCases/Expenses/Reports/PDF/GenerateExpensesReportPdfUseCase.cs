
using CashFlow.Application.UseCases.Expenses.Reports.PDF.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.PDF.Fonts;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace CashFlow.Application.UseCases.Expenses.Reports.PDF;

public class GenerateExpensesReportPdfUseCase : IGenerateExpensesReportPdfUseCase
{
    private readonly IExpensesReadOnlyRepository _repository;
    private const int HEIGHT_ROW_EXPENSE_TABLE = 25;
    private const string CURRENCY_SYMBOL = "€";


    public GenerateExpensesReportPdfUseCase(IExpensesReadOnlyRepository repository)
    {
        _repository = repository;

        GlobalFontSettings.FontResolver = new ExpensesReportFontsResolver();
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _repository.FilterByMonth(month);
        if (expenses.Count == 0)
        {
            return [];
        }

        var document = CreateDocument(month);// cria o documento
        var page = CreatePage(document);

        CreateHeaderWithProfilePhotoAndName(page);
        var totalExpenses = expenses.Sum(expense => expense.Amount); // vai somar as despesas
        CreateTotalExpenseSection(page, month, totalExpenses);

        foreach (var expense in expenses)
        {
            var table = CreateExpenseTable(page); //a cada despesa que percorrer, vai adicionar uma tabela nova.

            var row = table.AddRow(); // adiciona uma nova linha 
            row.Height = HEIGHT_ROW_EXPENSE_TABLE; //defini a altura da linha

            AddExpenseTitle(row.Cells[0], expense.Title);
            AddHeaderForAmounet(row.Cells[3]);

            row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;

            SetStyleBaseForExpenseInformation(row.Cells[0]);
            row.Cells[0].AddParagraph(expense.Date.ToString("D"));
            row.Cells[0].Format.LeftIndent = 20;

            SetStyleBaseForExpenseInformation(row.Cells[1]);
            row.Cells[1].AddParagraph(expense.Date.ToString("t"));


            SetStyleBaseForExpenseInformation(row.Cells[2]);
            row.Cells[2].AddParagraph(expense.paymentType.PaymentTypeToString());

            AddAmountForExpense(row.Cells[3], expense.Amount);

            if (string.IsNullOrWhiteSpace(expense.Description) == false) //vai verificar se a descriação é nula ou apenas com espaços
            {
                var descriptionRow = table.AddRow();
                descriptionRow.Height = HEIGHT_ROW_EXPENSE_TABLE;
               
                descriptionRow.Cells[0].Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 10, Color = ColorHelper.BLACK }; // estilizando a celula
                descriptionRow.Cells[0].Shading.Color = ColorHelper.GREEN_LIGHT; // background
                descriptionRow.Cells[0].VerticalAlignment = VerticalAlignment.Center; //alinha no centro da vertical
                descriptionRow.Cells[0].Format.LeftIndent = 20; //identeação a esquerda do texto para não ficar colado na borda
                descriptionRow.Cells[0].MergeRight = 2; // mescla a coluna [0] com [1] e [2]
                descriptionRow.Cells[0].AddParagraph(expense.Description);
                row.Cells[3].MergeDown = 1;
            }


            AddWhiteSpace(table);

        }
        return RenderDocument(document);
    }
    private Document CreateDocument(DateOnly month)
    {

        var document = new Document();
        document.Info.Title = $"{ResourcerReportGenerationMessages.EXPENSE_FOR} {month:Y}"; // Y - retornar por ex. june 2009
        document.Info.Author = "Luis Henrique";

        var style = document.Styles["normal"]; //estilo padrão
        style!.Font.Name = FontHelper.DEFAULT_FONT; //caso nãoe specificar a fonte desejada para o paragrafo, em todo o documento a fonte default será essa fonte.

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection(); //criando uma página
        section.PageSetup = document.DefaultPageSetup.Clone(); //vai criar um clone das páginas no documento
        section.PageSetup.PageFormat = PageFormat.A4;//FORMATO DA PÁGINA

        // ESTILIZAÇÕES DE MARGIN
        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;

        return section;
    }

    private void CreateHeaderWithProfilePhotoAndName(Section page)
    {
        var table = page.AddTable();
        table.AddColumn(); // uma coluna para a foto
        table.AddColumn("300"); // outra coluna para o texto ex: Olá, Luis com parametro responsável pelos px da largura

        var row = table.AddRow(); //adiciona uma linha na tabela 

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location); // vai passar a localização do projeto
        var pathFile = Path.Combine(directoryName!, "logo", "Picture.png"); // vai pegar a imagem localizada no projeto
        row.Cells[0].AddImage(pathFile); //vai adicionar a imagem no indice 0 da tabela 
        row.Cells[1].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 16 }; //estilizando o indice que conterá o texto
        row.Cells[1].AddParagraph("Olá, Luis Developer"); //texto que será preenchido no indice 1 da tabela
        row.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center; // alinhando no centro da tabela. 
    }
    private void CreateTotalExpenseSection(Section page, DateOnly month, decimal totalExpenses)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = "40"; // espaço anntes do titulo
        paragraph.Format.SpaceAfter = "40"; // espaço depois do titulo
        var title = string.Format(ResourcerReportGenerationMessages.TOTAL_SPENT_IN, month.ToString("Y")); //armazena na variavel o texto que será exibido

        paragraph.AddFormattedText(title, new Font { Name = FontHelper.RALEWAY_REGULAR, Size = 15 }); //adiciona o texto formatando ele
        paragraph.AddLineBreak();  //quebra de linha

        paragraph.AddFormattedText($"{totalExpenses} {CURRENCY_SYMBOL}", new Font { Name = FontHelper.WORKSANS_BLACK, Size = 50 }); // cria o texto com o valor total das despesa e simbolo da moida
    }

    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();
        table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left; // colum com a largura e texto formatado a esquerda
        table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;
        return table;
    }

    private void AddExpenseTitle(Cell cell, string expenseTitle)
    {
        cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorHelper.BLACK }; // estilizando a celula
        cell.Shading.Color = ColorHelper.RED_LIGHT; // background
        cell.VerticalAlignment = VerticalAlignment.Center; //alinha no centro da vertical
        cell.Format.LeftIndent = 20; //identeação a esquerda do texto para não ficar colado na borda
        cell.AddParagraph(expenseTitle); //adiciona o titulo da despesa
        cell.MergeRight = 2; // mescla a coluna [0] com [1] e [2]
    }

    private void AddHeaderForAmounet(Cell cell)
    {
        cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorHelper.WITHE };
        cell.Shading.Color = ColorHelper.RED_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.AddParagraph(ResourcerReportGenerationMessages.AMOUNT);
    }

    private void SetStyleBaseForExpenseInformation(Cell cell)
    {
        cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorHelper.BLACK };
        cell.Shading.Color = ColorHelper.GREEN_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddAmountForExpense(Cell cell, decimal amount)
    {
        cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 14, Color = ColorHelper.BLACK };
        cell.Shading.Color = ColorHelper.WITHE;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.AddParagraph($" -{amount} {CURRENCY_SYMBOL}");
    }
    private void AddWhiteSpace(Table table)
    {

        var row = table.AddRow(); //adiciona a linhga em branco
        row.Height = 30;
        row.Borders.Visible = false; //tira a borda
    }


    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document,
        };
        renderer.RenderDocument(); //vai rederinzar todo documento.

        using var file = new MemoryStream(); //vai salvar o documento na mémoria da máquina
        renderer.PdfDocument.Save(file);

        return file.ToArray(); //vai devolver o array de bytes
    }
}
