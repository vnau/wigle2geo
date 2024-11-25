# WiGle2Geo

WiGle2Geo is a project offering a **console application** and a **web application** for processing and visualizing GeoJSON data from WiGLE backups. It helps users filter and retrieve Wi-Fi and bluetooth network data, including details about SSIDs, BSSIDs, locations, and more. All outputs are in **GeoJSON** format, making it easy to integrate with mapping tools and GIS applications.

## Features

- Retrieve filtered Wi-Fi network data in GeoJSON format.
- Query specific locations for given BSSIDs.
- Flexible filtering by SSID, BSSID, type, capabilities, distance, and number of points and advanced filtering (e.g., time, duration, and distance ranges).
- GeoJSON output for direct integration with mapping tools.

## Console Application

### Usage

Navigate to the CLI directory and build the application:

```bash
dotnet build
```

Run the CLI application with the following options:

### Commands

#### `network` Retrieve Wi-Fi networks filtered by various criteria and output the results as GeoJSON.

```bash
dotnet run -- network --source <path_to_wigle_backup> [options]
```

##### Options:

- --source (required): Path to the WiGLE backup database.
- --ssid: Filter by SSID(s), separate multiple values with |.
- --bssid: Filter by BSSID(s), separate multiple values with |.
- --type: Filter by network type(s), use | for multiple types.
- --capabilities: Filter by network capabilities.
- --distanceGt: Filter networks with a distance greater than the specified value (meters).
- --distanceLt: Filter networks with a distance less than the specified value (meters).
- --locationsGt: Filter networks with more than the specified number of locations.
- --locationsLt: Filter networks with fewer than the specified number of locations.

##### Example:

```bash
dotnet run -- network --source ../backup.sqlite --ssid MyWiFi --distanceGt 50
```

The output will be a GeoJSON FeatureCollection with matching network data.

#### `location` Retrieve location data for a specific BSSID and output the results as GeoJSON.

```bash
dotnet run -- location --source <path_to_wigle_backup> --bssid <bssid_value>
```

##### Options:

- --source (required): Path to the WiGLE backup database.
- --bssid (required): BSSID to retrieve location data for.

##### Example:

```bash
dotnet run -- location --source ../backup.sqlite --bssid 00:14:22:01:23:45
```

## Web Application

### API Endpoints

The web application exposes RESTful endpoints for querying GeoJSON data.

#### `GET /geo/network` Retrieve Wi-Fi networks filtered by query parameters. Results are returned as a GeoJSON FeatureCollection.

##### Query Parameters:

- ssid: Filter by SSID(s), separated by |.
- bssid: Filter by BSSID(s), separated by |.
- type: Filter by network type(s), separated by |.
- capabilities: Filter by network capabilities.
- vendor: Filter by vendor.
- distance[gt]: Filter networks with distance greater than the specified value (meters).
- distance[lt]: Filter networks with distance less than the specified value (meters).
- locations[gt]: Filter networks with more than the specified number of locations.
- locations[lt]: Filter networks with fewer than the specified number of locations.
- time[gt]: Filter networks recorded after the specified time (ISO 8601 or Unix timestamp).
- time[lt]: Filter networks recorded before the specified time (ISO 8601 or Unix timestamp).
- duration[gt]: Filter networks with duration greater than the specified value.
- duration[lt]: Filter networks with duration less than the specified value.

##### Example:

```bash
curl "http://localhost:5000/geo/network?ssid=MyWiFi&type=W&distance[gt]=50"
```

Output: A GeoJSON FeatureCollection containing the matching network features.

#### `GET /geo/location` Retrieve location data for a specific BSSID. Results are returned as a GeoJSON FeatureCollection.

##### Query Parameters:

- bssid (required): Filter by BSSID.

##### Example:

```bash
curl "http://localhost:5000/geo/location?bssid=00:14:22:01:23:45"
```

Output: A GeoJSON FeatureCollection containing the matching location features.
