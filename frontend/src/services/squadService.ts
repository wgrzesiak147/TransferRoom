import axios from "axios";
import type { Squad } from "../models/Squad";

const API_BASE_URL = "http://localhost:5231/api";

export async function fetchSquad(
  team: string,
  season?: number
): Promise<Squad> {
  const url = new URL(`${API_BASE_URL}/squad`);
  url.searchParams.append("team", team);
  if (season) {
    url.searchParams.append("season", season.toString());
  }

  const response = await axios.get<Squad>(url.toString());
  return response.data;
}
