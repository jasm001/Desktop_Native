import {
  apiVersion,
  type ApiErrorCode,
} from "@it-support-native/control-plane-contracts";
import { NextResponse } from "next/server";
import { ZodError } from "zod";
import { ApplicationError } from "../errors/application-error";

export function successResponse<T>(
  data: T,
  correlationId: string,
  status = 200,
): NextResponse {
  return NextResponse.json(
    {
      data,
      meta: { apiVersion, correlationId },
    },
    { status },
  );
}

export function errorResponse(
  error: unknown,
  correlationId: string,
): NextResponse {
  if (error instanceof ApplicationError) {
    return createErrorResponse(
      error.code,
      error.message,
      correlationId,
      error.httpStatus,
    );
  }

  if (error instanceof ZodError) {
    return createErrorResponse(
      "invalid_request",
      "The request is invalid.",
      correlationId,
      400,
    );
  }

  return createErrorResponse(
    "internal_error",
    "The request could not be completed.",
    correlationId,
    500,
  );
}

function createErrorResponse(
  code: ApiErrorCode,
  message: string,
  correlationId: string,
  status: number,
): NextResponse {
  return NextResponse.json(
    {
      error: { code, message },
      meta: { apiVersion, correlationId },
    },
    { status },
  );
}
