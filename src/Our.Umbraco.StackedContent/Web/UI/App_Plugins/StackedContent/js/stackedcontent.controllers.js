angular.module("umbraco").controller("Our.Umbraco.StackedContent.Controllers.StackedContentPropertyEditorController", [

    "$scope",
    "editorState",
    "notificationsService",
    "localStorageService",
    "innerContentService",
    "Our.Umbraco.StackedContent.Resources.StackedContentResources",

    function ($scope, editorState, notificationsService, localStorageService, innerContentService, scResources) {

        $scope.inited = false;
        $scope.markup = {};
        $scope.prompts = {};
        $scope.model.value = $scope.model.value || [];

        $scope.contentTypeGuids = _.uniq($scope.model.config.contentTypes.map(function (itm) {
            return itm.icContentTypeGuid;
        }));

        $scope.canAdd = function () {
            return (!$scope.model.config.maxItems || $scope.model.config.maxItems === "0" || $scope.model.value.length < $scope.model.config.maxItems) && $scope.model.config.singleItemMode !== "1";
        };

        $scope.canDelete = function () {
            return $scope.model.config.singleItemMode !== "1";
        };

        $scope.canCopy = function () {
            // TODO: Move this to InnerContent Service
            return localStorageService.isSupported; // innerContentService.canCopyContent();
        };

        $scope.canPaste = function () {
            //if (innerContentService.canPasteContent() && $scope.canAdd()) {
            if (localStorageService.isSupported && $scope.canAdd()) {
                return allowPaste;
            }
            return false;
        };

        $scope.addContent = function (evt, idx) {
            $scope.overlayConfig.event = evt;
            $scope.overlayConfig.data = { model: null, idx: idx, action: "add" };
            $scope.overlayConfig.show = true;
        };

        $scope.editContent = function (evt, idx, itm) {
            $scope.overlayConfig.event = evt;
            $scope.overlayConfig.data = { model: itm, idx: idx, action: "edit" };
            $scope.overlayConfig.show = true;
        };

        $scope.deleteContent = function (evt, idx) {
            $scope.model.value.splice(idx, 1);
            setDirty();
        };

        $scope.copyContent = function (evt, idx) {
            var item = $scope.model.value[idx];
            // TODO: Move this to InnerContent Service
            // var success = innerContentService.setCopiedContent(item);
            // if (success) {
            if (item && item.icContentTypeGuid) {
                localStorageService.set("icContentJson", JSON.stringify(item, function (k, v) {
                    if (k === "key" || k === "$$hashKey") {
                        return undefined;
                    }
                    return v;
                }));
                allowPaste = true;
                notificationsService.success("Content", "The content block has been copied.");
            } else {
                notificationsService.error("Content", "Unfortunately, the content block was not able to be copied.");
            }
        };

        $scope.pasteContent = function (evt, idx) {
            // TODO: Move this to InnerContent Service
            // var item = innerContentService.getCopiedContent();
            var item = JSON.parse(localStorageService.get("icContentJson"));
            item.key = innerContentService.generateUid();

            if (item && contentTypeGuidIsAllowed(item.icContentTypeGuid)) {
                $scope.overlayConfig.callback({ model: item, idx: idx, action: "add" });
                setDirty();
            } else {
                notificationsService.error("Content", "Unfortunately, the content block is not allowed to be pasted here.");
            }
        };

        $scope.sortableOptions = {
            axis: 'y',
            cursor: "move",
            handle: ".stack__preview-wrapper",
            helper: function () {
                return $('<div class=\"stack__sortable-helper\"><div><i class=\"icon icon-navigation\"></i></div></div>');
            },
            cursorAt: {
                top: 0
            },
            stop: function (e, ui) {
                _.each($scope.model.value, function (itm, idx) {
                    innerContentService.populateName(itm, idx, $scope.model.config.contentTypes);
                });
                setDirty();
            }
        };

        // Helpers
        var loadPreviews = function () {
            _.each($scope.model.value, function (itm) {
                scResources.getPreviewMarkup(itm, editorState.current.id).then(function (markup) {
                    if (markup) {
                        $scope.markup[itm.key] = markup;
                    }
                });
            });
        };

        var previewEnabled = function () {
            return $scope.model.config.disablePreview !== "1";
        };

        var setDirty = function () {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        var contentTypeGuidIsAllowed = function (guid) {
            return !!guid && _.contains($scope.contentTypeGuids, guid);
        };

        var pasteAllowed = function () {
            // TODO: Move this to InnerContent Service
            // var guid = innerContentService.getCopiedContentTypeGuid();
            var json = localStorageService.get("icContentJson");
            if (json !== null) {
                var item = JSON.parse(json);
                return item && contentTypeGuidIsAllowed(item.icContentTypeGuid);
            }
            return false;
        };

        // Storing the 'canPaste' check in a local variable, so that it doesn't need to be re-eval'd every time
        var allowPaste = pasteAllowed();

        // Set overlay config
        $scope.overlayConfig = {
            propertyAlias: $scope.model.alias,
            contentTypes: $scope.model.config.contentTypes,
            show: false,
            data: {
                idx: 0,
                model: null
            },
            callback: function (data) {
                innerContentService.populateName(data.model, data.idx, $scope.model.config.contentTypes);

                if (previewEnabled()) {
                    scResources.getPreviewMarkup(data.model, editorState.current.id).then(function (markup) {
                        if (markup) {
                            $scope.markup[data.model.key] = markup;
                        }
                    });
                }

                if (!($scope.model.value instanceof Array)) {
                    $scope.model.value = [];
                }

                if (data.action === "add") {
                    $scope.model.value.splice(data.idx, 0, data.model);
                } else if (data.action === "edit") {
                    $scope.model.value[data.idx] = data.model;
                }
            }
        };

        // Initialize value
        if ($scope.model.value.length > 0) {

            // Model is ready so set inited
            $scope.inited = true;

            // Sync icons incase it's changes on the doctype
            var guids = _.uniq($scope.model.value.map(function (itm) {
                return itm.icContentTypeGuid;
            }));

            innerContentService.getContentTypeIconsByGuid(guids).then(function (data) {
                _.each($scope.model.value, function (itm) {
                    if (data.hasOwnProperty(itm.icContentTypeGuid)) {
                        itm.icon = data[itm.icContentTypeGuid];
                    }
                });

                // Try loading previews
                if (previewEnabled()) {
                    loadPreviews();
                }
            });

        } else if ($scope.model.config.singleItemMode === "1") {

            // Initialise single item mode model
            innerContentService.createDefaultDbModel($scope.model.config.contentTypes[0]).then(function (v) {

                $scope.model.value = [v];

                // Model is ready so set inited
                $scope.inited = true;

                // Try loading previews
                if (previewEnabled()) {
                    loadPreviews();
                }

            });

        } else {

            // Model is ready so set inited
            $scope.inited = true;

        }
    }

]);
