document.addEventListener("DOMContentLoaded", function () {
    const svg = document.querySelector("svg");
    const areaName = svg.getAttribute("data-area-name");
    const statusCookieName = `status_${areaName}`;

    // Load status from cookie
    const statusCookie = getCookie(statusCookieName);
    if (statusCookie) {
        const statusData = JSON.parse(statusCookie);
        statusData.forEach(nodeId => {
            const node = document.getElementById(nodeId);
            if (node) {
                node.setAttribute("Status", "on");
                node.classList.add("status-on");
            }
        });
    }
    // Ensure pointer-events are enabled for the SVG elements
    document.querySelectorAll('g.node').forEach(node => {
        node.setAttribute('pointer-events', 'all');
    });


    // Toggle status on click
    svg.addEventListener("click", function (event) {
        console.log("Click event detected"); // Debugging log
        const node = event.target.closest("g.node");
        console.log("Closest node:", node); // Debugging log
        if (node) {
            const status = node.getAttribute("Status");
            if (status === "off") {
                node.setAttribute("Status", "on");
                node.classList.add("status-on");
            } else {
                node.setAttribute("Status", "off");
                node.classList.remove("status-on");
            }
            updateStatusCookie();
        }
    });

    // Update status cookie
    function updateStatusCookie() {
        const nodes = svg.querySelectorAll("g.node[Status='on']");
        const statusData = Array.from(nodes).map(node => node.getAttribute("ID"));
        setCookie(statusCookieName, JSON.stringify(statusData), 365);
    }

    // Get cookie value
    function getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(";").shift();
    }

    // Set cookie value
    function setCookie(name, value, days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        const expires = `expires=${date.toUTCString()}`;
        document.cookie = `${name}=${value};${expires};path=/`;
    }
});

function performDepthSearch() {
    const depth = parseInt(document.getElementById("depthSelect").value);
    const svg = document.querySelector("svg");
    const nodesOn = svg.querySelectorAll("g.node[Status='on']");
    const relationships = JSON.parse(svg.getAttribute("data-relationships"));

    let items = {};

    nodesOn.forEach(node => {
        const nodeId = parseInt(node.getAttribute("ID"));
        const foundNodes = depthSearch(nodeId, depth, relationships);
        foundNodes.forEach(foundNode => {
            const foundNodeElement = document.querySelector(`g.node[ID='${foundNode}']`);
            if (foundNodeElement && foundNodeElement.getAttribute("Status") === "off") {
                const itemsAttribute = foundNodeElement.getAttribute("Items");
                const itemsArray = parseItems(itemsAttribute);
                itemsArray.forEach(item => {
                    if (items[item.name]) {
                        items[item.name] += item.qty;
                    } else {
                        items[item.name] = item.qty;
                    }
                });
            }
        });
    });

    displayItems(items);
}

function depthSearch(nodeId, depth, relationships) {
    let foundNodes = [];
    let queue = [{ nodeId, depth }];

    while (queue.length > 0) {
        const { nodeId, depth } = queue.shift();
        if (depth > 0) {
            relationships.forEach(rel => {
                if (rel.from === nodeId) {
                    foundNodes.push(rel.to);
                    queue.push({ nodeId: rel.to, depth: depth - 1 });
                }
            });
        }
    }

    return foundNodes;
}

function parseItems(itemsAttribute) {
    return itemsAttribute.split(", ").map(item => {
        const parts = item.split(", qty: ");
        return { name: parts[0].replace("item: ", ""), qty: parseInt(parts[1]) };
    });
}

function displayItems(items) {
    let itemList = "";
    for (const [name, qty] of Object.entries(items)) {
        itemList += `${name}: ${qty}\n`;
    }
    alert(itemList);
}
