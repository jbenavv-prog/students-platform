import { SubjectSummary } from '../../features/students/models/student.models';

export interface SelectionAnalysis {
  selectedSubjects: SubjectSummary[];
  selectedProfessors: string[];
  totalCredits: number;
  valid: boolean;
  messages: string[];
}

export function analyzeSubjectSelection(
  subjectIds: readonly string[],
  subjects: readonly SubjectSummary[]
): SelectionAnalysis {
  const uniqueIds = [...new Set(subjectIds)];
  const selectedSubjects = uniqueIds
    .map((subjectId) => subjects.find((subject) => subject.id === subjectId))
    .filter((subject): subject is SubjectSummary => Boolean(subject));

  const messages: string[] = [];

  if (subjectIds.length !== uniqueIds.length) {
    messages.push('No puedes repetir una materia.');
  }

  if (uniqueIds.length > 3) {
    messages.push('Solo puedes seleccionar hasta 3 materias.');
  }

  const professorIds = new Set<string>();
  const selectedProfessors: string[] = [];

  for (const subject of selectedSubjects) {
    if (professorIds.has(subject.professorId)) {
      messages.push('No puedes seleccionar dos materias dictadas por el mismo profesor.');
      continue;
    }

    professorIds.add(subject.professorId);
    selectedProfessors.push(subject.professorName);
  }

  const totalCredits = selectedSubjects.reduce((sum, subject) => sum + subject.credits, 0);

  if (totalCredits > 9) {
    messages.push('El maximo de creditos por estudiante es 9.');
  }

  if (uniqueIds.length !== 3) {
    messages.push('Debes seleccionar exactamente 3 materias.');
  }

  return {
    selectedSubjects,
    selectedProfessors,
    totalCredits,
    valid: messages.length === 0,
    messages: [...new Set(messages)]
  };
}

export function canAddSubject(
  subjectId: string,
  selectedSubjectIds: readonly string[],
  subjects: readonly SubjectSummary[]
): boolean {
  if (selectedSubjectIds.includes(subjectId)) {
    return true;
  }

  if (selectedSubjectIds.length >= 3) {
    return false;
  }

  const currentAnalysis = analyzeSubjectSelection(selectedSubjectIds, subjects);
  const subject = subjects.find((item) => item.id === subjectId);

  if (!subject) {
    return false;
  }

  return !currentAnalysis.selectedSubjects.some(
    (selectedSubject) => selectedSubject.professorId === subject.professorId
  );
}
