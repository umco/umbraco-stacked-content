angular.module('umbraco.resources').factory('Our.Umbraco.StackedContent.Resources.StackedContentResources',
    function ($q, $http, umbRequestHelper) {
        return {
            getPreviewMarkup: function (data, pageId) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: "/umbraco/backoffice/StackedContent/StackedContentApi/GetPreviewMarkup",
                        method: "POST",
                        params: { pageId: pageId },
                        data: data
                    }),
                    'Failed to retrieve preview markup'
                );
            }
        };
    });