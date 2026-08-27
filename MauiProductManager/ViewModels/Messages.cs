using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiProductManager.Models;

namespace MauiProductManager.ViewModels;

public record ProductUpdatedMessage(Product Product);

public record ProductDeletedMessage(int ProductId);
