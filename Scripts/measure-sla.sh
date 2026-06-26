#!/usr/bin/env bash
set -euo pipefail

booking_url="${BOOKING_URL:-http://localhost:5000}"
requests="${REQUESTS:-20}"
max_ms="${MAX_MS:-2000}"
target_success="${TARGET_SUCCESS:-99}"

now_ms() {
  perl -MTime::HiRes=time -e 'printf "%d\n", time() * 1000'
}

extract_id() {
  sed -n 's/.*"id":\([0-9][0-9]*\).*/\1/p'
}

success_count=0
failure_count=0
durations=()

for ((i = 1; i <= requests; i++)); do
  seat_id=$((10000 + i))
  started_at="$(now_ms)"

  booking_response="$(
    curl -sS \
      -H "Content-Type: application/json" \
      -d "{\"clientId\":1,\"eventId\":1,\"seatIds\":[${seat_id}]}" \
      "${booking_url}/booking/add"
  )"

  booking_id="$(printf "%s" "${booking_response}" | extract_id)"

  if [[ -n "${booking_id}" ]]; then
    pay_status="$(
      curl -sS -o /dev/null -w "%{http_code}" \
        -X POST \
        "${booking_url}/booking/pay/${booking_id}?cost=100"
    )"
  else
    pay_status="000"
  fi

  finished_at="$(now_ms)"
  duration=$((finished_at - started_at))
  durations+=("${duration}")

  if [[ "${pay_status}" == "200" && "${duration}" -le "${max_ms}" ]]; then
    success_count=$((success_count + 1))
  else
    failure_count=$((failure_count + 1))
  fi
done

sorted_durations="$(printf "%s\n" "${durations[@]}" | sort -n)"
p95_index=$(((requests * 95 + 99) / 100))
p95="$(printf "%s\n" "${sorted_durations}" | sed -n "${p95_index}p")"
success_rate="$(awk "BEGIN { printf \"%.2f\", (${success_count} / ${requests}) * 100 }")"

printf "| metric | value |\n"
printf "| --- | ---: |\n"
printf "| requests | %s |\n" "${requests}"
printf "| success within %sms | %s |\n" "${max_ms}" "${success_count}"
printf "| failures | %s |\n" "${failure_count}"
printf "| success rate | %s%% |\n" "${success_rate}"
printf "| p95 latency | %sms |\n" "${p95}"
printf "| SLA target | >= %s%% within %sms |\n" "${target_success}" "${max_ms}"
