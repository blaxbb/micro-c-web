const db = idb.openDB("micro-c-cache", 4, {
    upgrade(db, oldVersion, newVersion, transaction) {
        console.log("upgrade");
        var itemStore = db.createObjectStore("items", { keyPath: "sku" });
        itemStore.createIndex("Category", "category", { unique: false });
        itemStore.createIndex("Name", "name", { unique: false });

        var catStore = db.createObjectStore("categories", { keyPath: "category" });
        catStore.createIndex("Created", "created", { unique: false });
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

async function GetCacheItemByCategory(category) {
    var res = await (await db).getAllFromIndex("items", "Category", category);
    return res;
}

async function AddCacheCategory(category) {
    (await db).add("categories", category);
}

async function GetCacheCategory(category) {
    return (await db).get('categories', category);
}