import axios from "../axios";

export async function shortenUrl(originalUrl) {
    try {
        const res = await axios.post("/ShortUrl", { originalUrl });
        return res.data;
    } catch (err) {
        console.error("Shorten failed:", err.response?.data || err.message);
        alert(
            "Failed to shorten URL: " +
            (err.response?.data?.message ||
                JSON.stringify(err.response?.data) ||
                err.message)
        );
        throw err;
    }
}

export async function getMyUrls() {
    try {
        const res = await axios.get("/ShortUrl");
        return res.data;
    } catch (err) {
        console.error("Get URLs failed:", err.response?.data || err.message);
        throw err;
    }
}
