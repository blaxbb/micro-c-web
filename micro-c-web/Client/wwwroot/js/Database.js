const db = idb.openDB("micro-c-cache", 3, {
    upgrade(db, oldVersion, newVersion, transaction) {
        console.log("upgrade");
        var store = db.createObjectStore("items", { keyPath: "sku" });
        store.createIndex("Name", "name", { unique: false });
        store.createIndex("Category", "category", { unique: false })
    },
    blocked() {

    },
    blocking() {

    },
    terminated() {

    }
});

async function AddCacheItem(item) {
    (await db).add("items", item);
}

async function GetCacheItem(sku) {
    return (await db).get('items', sku);
}

async function GetCacheCategory(category) {
    var res = await (await db).getAllFromIndex("items", "Category", category);
    debugger;
    return res;
}