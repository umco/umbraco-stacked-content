angular.module("umbraco").controller("Our.Umbraco.StackedContent.Controllers.StackedContentPropertyEditorController", [
    "$scope",
    "editorState",
    "notificationsService",
    "innerContentService",
    "Our.Umbraco.StackedContent.Resources.StackedContentResources",
    function ($scope, editorState, notificationsService, innerContentService, scResources) {

        var defaultConfig = {
            contentTypes: [],
            enableCopy: 1,
            enableFilter: 0,
            enablePreview: 0,
            singleItemMode: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            config.previewEnabled = config.enablePreview === "1" || config.enablePreview === 1;
            config.copyEnabled = config.enableCopy === "1" || config.enableCopy === 1;

            vm.inited = false;
            vm.markup = {};
            vm.prompts = {};

            vm.singleItemMode = config.singleItemMode === "1" || config.singleItemMode === 1;

            $scope.model.value = $scope.model.value || [];

            if ($scope.model.value === "") {
                $scope.model.value = [];
            }

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            vm.sortableOptions = {
                axis: "y",
                cursor: "move",
                handle: ".stack__preview-wrapper",
                helper: function () {
                    return $("<div class=\"stack__sortable-helper\"><div><i class=\"icon icon-navigation\"></i></div></div>");
                },
                cursorAt: {
                    top: 0
                },
                stop: function (e, ui) {
                    $scope.model.value.forEach(function (x, idx) {
                        innerContentService.populateName(x, idx, config.contentTypes);
                    });
                    setDirty();
                }
            };

            // Set overlay config
            vm.overlayConfig = {
                propertyAlias: $scope.model.alias,
                contentTypes: config.contentTypes,
                enableFilter: config.enableFilter === "1" || config.enableFilter === 1,
                show: false,
                data: {
                    idx: 0,
                    model: null
                },
                callback: function (data) {
                    innerContentService.populateName(data.model, data.idx, config.contentTypes);

                    if (config.previewEnabled) {
                        scResources.getPreviewMarkup(data.model, editorState.current.id).then(function (markup) {
                            if (markup) {
                                vm.markup[data.model.key] = markup;
                            }
                        });
                    }

                    if (($scope.model.value instanceof Array) === false) {
                        $scope.model.value = [];
                    }

                    if (data.action === "add") {
                        $scope.model.value.splice(data.idx, 0, data.model);
                    } else if (data.action === "edit") {
                        $scope.model.value[data.idx] = data.model;
                    }
                }
            };

            vm.canAdd = canAdd;
            vm.canCopy = canCopy;
            vm.canDelete = canDelete;

            vm.addContent = addContent;
            vm.copyContent = copyContent;
            vm.deleteContent = deleteContent;
            vm.editContent = editContent;

            if ($scope.model.value.length > 0) {

                // Model is ready so set inited
                vm.inited = true;

                // Sync icons incase it's changes on the doctype
                var guids = _.uniq($scope.model.value.map(function (x) {
                    return x.icContentTypeGuid;
                }));

                innerContentService.getContentTypeIconsByGuid(guids).then(function (data) {
                    _.each($scope.model.value, function (x) {
                        if (data.hasOwnProperty(x.icContentTypeGuid)) {
                            x.icon = data[x.icContentTypeGuid];
                        }
                    });

                    // Try loading previews
                    if (config.previewEnabled) {
                        loadPreviews();
                    }
                });

            } else if (editorState.current.hasOwnProperty("contentTypeAlias") && vm.singleItemMode === true) {

                // Initialise single item mode model
                innerContentService.createDefaultDbModel(config.contentTypes[0]).then(function (v) {

                    $scope.model.value = [v];

                    // Model is ready so set inited
                    vm.inited = true;

                    // Try loading previews
                    if (config.previewEnabled) {
                        loadPreviews();
                    }

                });

            } else {

                // Model is ready so set inited
                vm.inited = true;

            }

        };

        function canAdd() {
            return (!config.maxItems || config.maxItems === "0" || $scope.model.value.length < config.maxItems) && vm.singleItemMode === false;
        };

        function canDelete() {
            return vm.singleItemMode === false;
        };

        function canCopy() {
            return config.copyEnabled && innerContentService.canCopyContent();
        };

        function addContent(idx) {
            vm.overlayConfig.data = { model: null, idx: idx, action: "add" };
            vm.overlayConfig.show = true;
        };

        function editContent(idx, itm) {
            vm.overlayConfig.data = { model: itm, idx: idx, action: "edit" };
            vm.overlayConfig.show = true;
        };

        function deleteContent(idx) {
            $scope.model.value.splice(idx, 1);
            setDirty();
        };

        function copyContent(idx) {
            var item = Object.assign({}, $scope.model.value[idx]);
            var success = innerContentService.setCopiedContent(item);
            if (success) {
                notificationsService.success("Content", "The content block has been copied.");
            } else {
                notificationsService.error("Content", "Unfortunately, the content block was not able to be copied.");
            }
        };

        function loadPreviews() {
            $scope.model.value.forEach(function (x) {
                scResources.getPreviewMarkup(x, editorState.current.id).then(function (markup) {
                    if (markup) {
                        vm.markup[x.key] = markup;
                    }
                });
            });
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);

angular.module("umbraco.resources").factory("Our.Umbraco.StackedContent.Resources.StackedContentResources", [
    "$http",
    "umbRequestHelper",
    function ($http, umbRequestHelper) {
        return {
            getPreviewMarkup: function (data, pageId) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.convertVirtualToAbsolutePath("~/umbraco/backoffice/StackedContent/StackedContentApi/GetPreviewMarkup"),
                        method: "POST",
                        params: { pageId: pageId },
                        data: data
                    }),
                    "Failed to retrieve preview markup"
                );
            }
        };
    }
]);
