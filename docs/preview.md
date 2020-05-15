# Stacked Content

## Previewing blocks

Preview looks for partials in this folder "~/Views/Partials/Stack/", with a matching DocTypeAlias filename.

Partials should inherit a [`PreviewModel`](https://github.com/umco/umbraco-stacked-content/blob/develop/src/Our.Umbraco.StackedContent/Web/PreviewModel.cs) model. It contains props for the Item (block) and containing Page.
