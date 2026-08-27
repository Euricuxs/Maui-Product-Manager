# Android Local Development

This guide explains how to connect the MAUI Android application to the ASP.NET Core Web API running on your Windows development PC, using either a physical Android device or an Android emulator.

---

## 1. Start the ASP.NET Core API

```bash
cd MauiProductManager.Api
dotnet restore
dotnet run
```

On first run, the API automatically applies database migrations and creates `products.db`. The API starts at `http://localhost:5000`.

---

## 2. Find Your PC's Local IPv4 Address

On Windows, open a command prompt and run:

```bash
ipconfig
```

Look for the **IPv4 Address** under your active network adapter (Wi-Fi or Ethernet). For example:

```
Wireless LAN adapter Wi-Fi:
   IPv4 Address. . . . . . . . . . . : 192.168.101.70
```

Your IP address is `192.168.101.70`. Replace this with your actual IP address in all configurations below.

---

## 3. Find the API HTTP Port

The API runs on **port 5000** by default (Kestrel's default when no URLs are configured).

To verify, open a browser on your Windows PC and navigate to:

```
http://localhost:5000/swagger
```

---

## 4. Configure the MAUI API Base URL

The MAUI app reads its API base URL from `MauiProductManager/appsettings.json`. This file is bundled with the app.

### Android Emulator

By default, the app is configured for emulator testing:

```json
{
  "ApiBaseUrl": "http://10.0.2.2:5000"
}
```

`10.0.2.2` is the Android emulator's built-in alias for the host machine's `localhost`.

### Physical Android Device

Edit `MauiProductManager/appsettings.json` and replace the IP with your PC's local IPv4 address:

```json
{
  "ApiBaseUrl": "http://192.168.101.70:5000"
}
```

Replace `192.168.101.70` with your actual PC IP address from step 2.

---

## 5. Network Requirements

### Phone and PC on the Same Network

Both the Android device and the Windows PC must be connected to the **same Wi-Fi network**. The PC's firewall must also allow inbound connections on port 5000 (see Firewall section below).

### Physical Android Device

The physical device communicates with the PC over the local Wi-Fi network using the PC's LAN IP address (e.g. `192.168.101.70:5000`). **Do not use `localhost` or `127.0.0.1`** on a physical device.

### Android Emulator

The emulator uses a virtual router. The host PC's `localhost` is accessible from the emulator at `10.0.2.2`.

| Environment | Base URL |
|---|---|
| Android emulator | `http://10.0.2.2:5000` |
| Physical Android device | `http://<your-pc-lan-ip>:5000` |
| Windows PC (browser/curl) | `http://localhost:5000` |

---

## 6. Verify API is Reachable

### From the Android Emulator

The emulator can reach the host machine at `10.0.2.2`. Open a browser in the emulator and navigate to:

```
http://10.0.2.2:5000/api/products
```

### From a Physical Android Device

Open a browser on the physical Android device and navigate to:

```
http://192.168.101.70:5000/api/products
```

Replace `192.168.101.70` with your actual PC IP.

If the page loads with JSON data, the connection is working. If not, see the Troubleshooting section below.

---

## 7. Run the MAUI Android Application

```bash
cd MauiProductManager
dotnet build -t:Run -f net10.0-android
```

The app should connect to the API automatically using the configured base URL from `appsettings.json`.

---

## 8. Test Product CRUD

1. **Load Products** — The product list should display all products from the API.
2. **Create Product** — Tap "+", fill in name/price/category, and save.
3. **Edit Product** — Tap a product, then tap "Edit", modify fields, and save.
4. **Delete Product** — Tap a product, then tap "Delete" and confirm.
5. **Search** — Use the search bar to filter by name or category.

---

## 9. Troubleshooting

### API returns empty product list

- Verify the API is running (`dotnet run` in `MauiProductManager.Api`)
- Verify the base URL in `appsettings.json` matches your PC's IP
- Check that the phone and PC are on the same Wi-Fi network

### Connection refused / unreachable

- Verify the API is running on port 5000
- Verify the base URL has the correct IP and port
- Check Windows Firewall rules (see below)

### localhost / 127.0.0.1 not working on physical device

This is expected. A physical Android device cannot reach `localhost` on the PC because they are separate machines. Use the PC's actual LAN IP address (e.g. `192.168.101.70:5000`).

### localhost not working on emulator

Use `10.0.2.2` instead of `localhost` for the emulator. `10.0.2.2` is the emulator's alias for the host machine.

### Different Wi-Fi networks

The PC and phone must be on the **same** Wi-Fi network. If the PC uses Ethernet and the phone uses Wi-Fi, they should still be on the same local network (same router/subnet).

### Wrong port

The API runs on port **5000** by default. If you changed the port, update the base URL accordingly.

### Windows Firewall

If the Android device (physical or emulator) cannot reach the API, Windows Firewall may be blocking inbound connections on port 5000.

**To allow the API through the firewall (development only):**

1. Open **Windows Defender Firewall** → **Advanced settings**
2. Click **Inbound Rules** → **New Rule**
3. Rule Type: **Port**
4. Protocol: **TCP**, Port: **5000**
5. Action: **Allow the connection**
6. Profile: Check **Domain**, **Private**, and **Public**
7. Name: `MauiProductManager API (Development)`

This is a **development-only** rule. It does not expose the API to the public internet.

> **Note for Android Emulator**: The emulator uses a virtual network (typically `10.0.2.0/24`). Even though the emulator can ping `10.0.2.2` successfully, Windows Firewall blocks incoming TCP connections on port 5000 from the emulator's virtual network unless this rule is created. If the emulator still cannot connect after creating the rule, try creating the rule with all profiles (Domain, Private, and Public) enabled.

### HTTP Cleartext

The app already has `android:usesCleartextTraffic="true"` set in `AndroidManifest.xml`, which allows HTTP (non-HTTPS) traffic. No additional configuration is needed for local development.

---

## Configuration Files

| File | Purpose |
|---|---|
| `MauiProductManager/appsettings.json` | MAUI app API base URL (edit this to change URL) |
| `MauiProductManager.Api/appsettings.json` | API database connection string (`Data Source=products.db`) |
| `MauiProductManager/Platforms/Android/AndroidManifest.xml` | Android network permissions and cleartext traffic settings |
