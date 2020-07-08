﻿    import WorkItemDetails from "../parts.optional/WorkItemDetails/WorkItemDetails";

    export default {
        // Arriba Service URL to query
        // Hardcode to the name of your service. Use port 42784 for HTTP, 42785 for HTTPS.
        url: "http://" + window.location.hostname + ":42784",

        // Name of tool to show [top right and elsewhere]
        toolName: "CSEng",

        // E-Mail addresses to whom feedback should go
        feedbackEmailAddresses: "slouvau@hotmail.com",

        // Listing: Default Columns and Sort Order to show per table
        listingDefaults: {
             "CSEng": { columns: ["ID", "Title", "Assigned To"], sortColumn: "ID", sortOrder: "desc" }
        },

        // Listing: Default details rendering per table
        customDetailsProviders: {
            "CSEng": WorkItemDetails
        },

        databaseName : "CSEng",
        directLinkUrl : "https://dev.azure.com/csefy19/_workitems/edit/",

        // Grid: Pre-defined Grid queries which appear in the 'Load' dropdown in the Grid view
        /* gridDefault: "<NameOfDefaultPredefinedQuery>", */
        gridDefaultQueries: {
            "Clear": {},
            "Arriba Tasks By Owner": {
                query: "Arriba AND [Remaining Work] > 0",
                aggregationFunction: "SUM",
                aggregateColumn: "Remaining Work",
                currentTable: "Louvau",
                rows: ["[Assigned To]>"],
                showPortionOf: "row"
            },
        },

        // Content to show before a query is typed
        startContent: {
            overview:(
                <span>
                    All Louvau work items indexed.
                    Need more <a href="Search.html?help=true">help</a>?
                </span>
            ),
            examples: {
                "[Area Path] : Arriba AND [Priority] <= 1 AND [State] != Closed": <span>Arriba Active</span>,
                "Arriba AND [Remaining Work] > 0": <span>Arriba Remaining Tasks</span>
            }
        },

        // Content to show if query fails due to access denied (401)
        accessDeniedContent: (
            <span>
                Access to CSEng denied.
            </span>
        ),

        // Content to show if query fails because service is unavailable (404, 500)
        serviceUnavailableContent: (
            <span>
                CSEng is currently unavailable.
            </span>
        )
    };
