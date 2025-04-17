
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chd.Base.UI.WPF.Extensions;
using chd.Poomsae.Scoring.WPF;
using chd.Poomsae.Scoring.WPF.Extensions;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);

Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddChdPoomsaeApp(builder.Configuration);
builder.Services.AddWPF<App>(builder.Configuration);


builder.Services.AddWpfBlazorWebView();
builder.Services.AddBlazorWebViewDeveloperTools();

var app = builder.Build();

app.Run();