const sleep = ms => new Promise(r => setTimeout(r, ms));
for(let i=0; i<20; i++){
    fetch("https://localhost:7298/item/95", {
        "body": "_handler=add-to-cart&__RequestVerificationToken=CfDJ8P4gA9PkMfhChjIBIo3MnIkzcfvna19EjENJ3sh81W0GnVjmOaRS0LpFMxMrMJVPUNFumjAXjNbe9MI6MEH3QSfVJD3IVecDiRveFrJqCISt07qTV22xLEVtWKYsnY0HWtf3XmhvN5w8f_PRvcd3ZNhAXOLDWbw-mUrOCev4X506FaNBUheAmvFAQompyUS5rw",
        "cache": "default",
        "credentials": "include",
        "headers": {
            "Accept": "text/html; blazor-enhanced-nav=on",
            "Accept-Language": "en-US,en;q=0.9",
            "Content-Type": "application/x-www-form-urlencoded",
            "Priority": "u=3, i",
            "User-Agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.3 Safari/605.1.15"
        },
        "method": "POST",
        "mode": "cors",
        "redirect": "follow",
        "referrer": "https://localhost:7298/item/95",
        "referrerPolicy": "strict-origin-when-cross-origin"
    })
    await sleep(1)
}