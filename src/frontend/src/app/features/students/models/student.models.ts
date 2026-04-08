export interface ProfessorSummary {
  id: string;
  fullName: string;
}

export interface SubjectSummary {
  id: string;
  name: string;
  credits: number;
  professorId: string;
  professorName: string;
}

export interface StudentListItem {
  id: string;
  fullName: string;
  email: string;
  programName: string;
  createdAt: string;
  subjectsCount: number;
  credits: number;
}

export interface EnrolledSubject {
  subjectId: string;
  subjectName: string;
  credits: number;
  professorId: string;
  professorName: string;
  classmates: string[];
}

export interface EnrollmentDetail {
  id: string;
  createdAt: string;
  totalCredits: number;
  subjects: EnrolledSubject[];
}

export interface StudentDetail {
  id: string;
  fullName: string;
  email: string;
  programName: string;
  createdAt: string;
  enrollment: EnrollmentDetail | null;
}

export interface StudentUpsertRequest {
  fullName: string;
  email: string;
  programName: string;
  subjectIds: string[];
}
